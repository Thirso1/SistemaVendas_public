using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVendas.Libraries.Email;
using SistemaVendas.Libraries.Filtro;
using SistemaVendas.Libraries.Lang;
using SistemaVendas.Libraries.Login;
using SistemaVendas.Libraries.Seguranca;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVendas.Areas.Colaborador.Controllers
{
    [Area("Colaborador")]
    public class HomeController : Controller
    {
        public DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        private IColaboradorRepository _repositoryColaborador;
        private LoginColaborador _loginColaborador;

        private IClienteRepository _clienteRepository;
        private IProdutoRepository _produtoRepository;
        private IVendaRepository _vendaRepository;
        //private GerenciarEmail _gerenciarEmail;

        public HomeController(
            IClienteRepository clienteRepository,
            IProdutoRepository produtoRepository,
            IColaboradorRepository repositoryColaborador,
            LoginColaborador loginColaborador,
            IVendaRepository vendaRepository
            //GerenciarEmail gerenciarEmail
        )
        {
            _repositoryColaborador = repositoryColaborador;
            _loginColaborador = loginColaborador;
            _clienteRepository = clienteRepository;
            _produtoRepository = produtoRepository;
            _vendaRepository = vendaRepository;
            //_gerenciarEmail = gerenciarEmail;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] Models.Colaborador colaborador)
        {
            Models.Colaborador colaboradorDB = _repositoryColaborador.Login(colaborador.Email, colaborador.Senha);

            if (colaboradorDB != null)
            {
                _loginColaborador.Login(colaboradorDB);

                return new RedirectResult(Url.Action(nameof(Painel)));
            }
            else
            {
                ViewData["MSG_E"] = "Usuário não encontrado, verifique o e-mail e senha digitado!";
                return View();
            }
        }

        [ColaboradorAutorizacao]
        //[ValidateHttpReferer]
        public IActionResult Logout()
        {
            _loginColaborador.Logout();
            return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recuperar([FromForm] Models.Colaborador colaborador)
        {
            //todo resolver email
            //var colaboradorDoBancoDados = _repositoryColaborador.ObterColaboradorPorEmail(colaborador.Email);

            //if (colaboradorDoBancoDados != null && colaboradorDoBancoDados.Count > 0)
            //{
            //    string idCrip = Base64Cipher.Base64Encode(colaboradorDoBancoDados.First().Id.ToString());
            //    _gerenciarEmail.EnviarLinkResetarSenha(colaboradorDoBancoDados.First(), idCrip);

            //    TempData["MSG_S"] = Mensagem.MSG_S004;

            //    ModelState.Clear();
            //}
            //else
            //{
            //    TempData["MSG_E"] = Mensagem.MSG_E014;
            //}


            return View();
        }

        [HttpGet]
        public IActionResult CriarSenha(string id)
        {
            try
            {
                var idColaboradorDecrip = Base64Cipher.Base64Decode(id);
                int idColaborador;
                if (!int.TryParse(idColaboradorDecrip, out idColaborador))
                {
                    TempData["MSG_E"] = Mensagem.MSG_E015;
                }
            }
            catch (System.FormatException)
            {
                TempData["MSG_E"] = Mensagem.MSG_E015;
            }

            return View();
        }

        [HttpPost]
        public IActionResult CriarSenha([FromForm] Models.Colaborador colaborador, string id)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Email");

            if (ModelState.IsValid)
            {
                int idColaborador;
                try
                {
                    var idColaboradorDecrip = Base64Cipher.Base64Decode(id);

                    if (!int.TryParse(idColaboradorDecrip, out idColaborador))
                    {
                        TempData["MSG_E"] = Mensagem.MSG_E015;
                        return View();
                    }



                }
                catch (System.FormatException)
                {
                    TempData["MSG_E"] = Mensagem.MSG_E015;
                    return View();
                }


                var colaboradorDB = _repositoryColaborador.ObterColaborador(idColaborador);
                if (colaboradorDB != null)
                {
                    colaboradorDB.Senha = colaborador.Senha;

                    _repositoryColaborador.AtualizarSenha(colaboradorDB);
                    TempData["MSG_S"] = Mensagem.MSG_S005;
                }

            }

            return View();
        }

        [ColaboradorAutorizacao]
        public IActionResult Painel()
        {
            decimal vendasHoje = _vendaRepository.VendasHoje();
            DateTime semana = _vendaRepository.Semana();

            decimal vendasSemana = _vendaRepository.VendasSemana();
            decimal vendasMes = _vendaRepository.VendasMes();

            decimal lucroHoje = _vendaRepository.LucroHoje();
            decimal lucroSemana = _vendaRepository.LucroSemana();
            decimal lucroMes = _vendaRepository.LucroMes();


            ViewBag.Semana = semana.ToString("yyyy-MM-dd");
            ViewBag.VendasHoje = vendasHoje.ToString("C");
            ViewBag.VendasSemana = vendasSemana.ToString("C");
            ViewBag.VendasMes = vendasMes.ToString("C");

            ViewBag.LucroHoje = lucroHoje.ToString("C");
            ViewBag.LucroSemana = lucroSemana.ToString("C");
            ViewBag.LucroMes = lucroMes.ToString("C");


            ViewBag.CustoHoje = (vendasHoje - lucroHoje).ToString("C");
            ViewBag.CustoSemana = (vendasSemana - lucroSemana).ToString("C");
            ViewBag.CustoMes = (vendasMes - lucroMes).ToString("C");

            ViewBag.Clientes = _clienteRepository.QuantidadeTotalClientes();
            ViewBag.Produto = _produtoRepository.QuantidadeTotalProdutos();
            ViewBag.Estoques = _produtoRepository.ativosEmEstoque().ToString("C");
            ViewBag.Prazo = _vendaRepository.TotalVendasPrazo().ToString("C");


            return View();
        }

        [ColaboradorAutorizacao]
        [HttpGet]
        public IActionResult Relatorio([FromQuery] string dataStr)
        {
            DateTime data = Convert.ToDateTime(dataStr);
            decimal vendasPorData = _vendaRepository.TotalPorData(data);
            decimal lucroPorData = _vendaRepository.LucroPorData(data);

            ViewBag.VendasPorData = vendasPorData.ToString("C");
            ViewBag.LucroPorData = lucroPorData.ToString("C");
            ViewBag.CustoPorData = (vendasPorData - lucroPorData).ToString("C");
            ViewBag.dataConsulta = data.ToString("yyyy-MM-dd");

            return View();
        }
        [ColaboradorAutorizacao]
        [HttpPost]
        public IActionResult Relatorio()
        {

            DateTime data = Convert.ToDateTime(Request.Form["dataConsulta"]);
            decimal vendasPorData = _vendaRepository.TotalPorData(data);
            decimal lucroPorData = _vendaRepository.LucroPorData(data);

            ViewBag.VendasPorData = vendasPorData.ToString("C");
            ViewBag.LucroPorData = lucroPorData.ToString("C");
            ViewBag.CustoPorData = (vendasPorData - lucroPorData).ToString("C");
            ViewBag.dataConsulta = data.ToString("yyyy-MM-dd");

            return View();
        }
    }
}