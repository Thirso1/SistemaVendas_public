using SistemaVendas.Libraries.Filtro;
using SistemaVendas.Libraries.Lang;
using SistemaVendas.Models;
using SistemaVendas.Models.Contants;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using X.PagedList;

namespace SistemaVendas.Areas.Colaborador.Controllers
{
    [Area("Colaborador")]
    [ColaboradorAutorizacao]
    public class ClienteController : Controller
    {
        private IClienteRepository _clienteRepository;
        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }
        public IActionResult Index(int? pagina, string pesquisa)
        {
            IPagedList<Models.Cliente> clientes = _clienteRepository.ObterTodosClientes(pagina, pesquisa);
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar([FromForm] Models.Cliente cliente, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                cliente.Situacao = SistemaVendas.Models.Contants.SituacaoConstant.Ativo;
                _clienteRepository.Cadastrar(cliente);
               // _loginCliente.Login(cliente);

                TempData["MSG_S"] = "Cadastro realizado com sucesso!";

                if (returnUrl == null)
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    return LocalRedirectPermanent(returnUrl);
                }
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Atualizar(int Id)
        {
            Models.Cliente cliente = _clienteRepository.ObterCliente(Id);

            return View(cliente);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.Situacao = SistemaVendas.Models.Contants.SituacaoConstant.Ativo;
                _clienteRepository.Atualizar(cliente);
                // _loginCliente.Login(cliente);

                TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        [ValidateHttpReferer]
        public IActionResult Excluir(int id)
        {
            _clienteRepository.Excluir(id);

            TempData["MSG_S"] = Mensagem.MSG_S002;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult BuscaClienteAjax(int id)
        {
            try
            {
                Cliente cliente = _clienteRepository.ObterCliente(id);

                string clienteJson = JsonConvert.SerializeObject(cliente);
                return Ok(clienteJson);
             
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}