using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SistemaVendas.Enums;
using SistemaVendas.Libraries.Filtro;
using SistemaVendas.Libraries.Lang;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.OData.Query.SemanticAst;
using Newtonsoft.Json;
using X.PagedList;

namespace SistemaVendas.Areas.Colaborador.Controllers
{
    [Area("Colaborador")]
    [ColaboradorAutorizacao]
    public class VendaController : Controller
    {
        private IVendaRepository _vendaRepository;
        private IItemVendaRepository _itemVendaRepository;
        private IClienteRepository _clienteRepository;
        private IProdutoRepository _produtoRepository;
        private IHttpContextAccessor _context;
        public DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
        public DateTime PegaDiaBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public VendaController(IVendaRepository vendaRepository, IHttpContextAccessor context, IClienteRepository clienteRepository, IProdutoRepository produtoRepository, IItemVendaRepository itemVendaRepository) 
        {
            _vendaRepository = vendaRepository;
            _context = context;
            _clienteRepository = clienteRepository;
            _produtoRepository = produtoRepository;
            _itemVendaRepository = itemVendaRepository;
        }      

        [HttpGet]                               
        public IActionResult Index(int? pagina, int selectClienteVenda,  string dataInicio, string dataFim, string statusVenda)
        {
            var testeRota = HttpContext.Request.PathBase.Value;


            List<DateTime> datas = resolveDatas(dataInicio, dataFim);
            bool consultaCliente = false;
            bool consultaStatusVenda = false;
            StatusPagamento statusPagamento = StatusPagamento.Pago;

            if (selectClienteVenda != 1 && selectClienteVenda != 0)
            {
                consultaCliente = true;
            }
            if (statusVenda != null)
            {
                consultaStatusVenda = true;
                statusPagamento = (StatusPagamento)Enum.Parse(typeof(StatusPagamento), statusVenda);
            }

            IPagedList<Venda> vendas;
            if (consultaCliente && !consultaStatusVenda)
            {
                vendas = _vendaRepository.ObterVendas(pagina, selectClienteVenda, datas[0], datas[1]);
            }
            else if (!consultaCliente && consultaStatusVenda)
            {
                vendas = _vendaRepository.ObterVendas(pagina, statusPagamento, datas[0], datas[1]);
            }
            else if (consultaCliente && consultaStatusVenda)
            {
                vendas = _vendaRepository.ObterVendas(pagina, selectClienteVenda, statusPagamento, datas[0], datas[1]);
            }
            else if (!consultaCliente && !consultaStatusVenda)
            {
                vendas = _vendaRepository.ObterVendas(pagina, datas[0], datas[1]);
            }
            else
            {
                vendas = _vendaRepository.ObterVendas(pagina, datas[0], datas[1]);
            }

            ViewBag.status = statusVenda;
            ViewBag.Cliente = _clienteRepository.ObterCliente(selectClienteVenda);
            ViewBag.inicio = datas[0].ToString("yyyy-MM-dd");
            ViewBag.fim = datas[1].ToString("yyyy-MM-dd"); 
            ViewBag.Clientes = _clienteRepository.ObterTodosClientes().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
            return View(vendas);
        }

        private List<DateTime> resolveDatas(string inicio, string fim)
        {
            List<DateTime> datas = new List<DateTime>();
            DateTime dataInicio;
            DateTime dataFim;
            if (String.IsNullOrEmpty(inicio))
            {
                dataInicio = PegaDiaBrasilia();
                dataInicio = dataInicio.AddDays(-30);
            }
            else
            {
                dataInicio = Convert.ToDateTime(inicio);
            }

            if (String.IsNullOrEmpty(fim))
            {
                dataFim = PegaDiaBrasilia();
                dataFim = dataFim.AddDays(1);
            }
            else
            {
                dataFim = Convert.ToDateTime(fim);
                dataFim = dataFim.AddDays(1);
            }
            datas.Add(dataInicio);
            datas.Add(dataFim);

            return datas;
        }

        [HttpGet]
        public IActionResult Cadastrar()
        { 
            Venda ultimaVenda = _vendaRepository.RetornaUltimaVenda();
            Venda novaVenda = new Venda();

            if (ultimaVenda.StatusVenda == StatusVenda.Iniciada)
            {
                novaVenda = ultimaVenda;
                novaVenda.DataRegistro = PegaHoraBrasilia();
                novaVenda.DataPagamento = novaVenda.DataRegistro;
            }
            else if(ultimaVenda.StatusVenda == StatusVenda.Suspensa)
            {
                novaVenda.Id = ultimaVenda.Id + 1;
                novaVenda.ClienteId = 1;
                novaVenda.DataRegistro = PegaHoraBrasilia();
                novaVenda.DataPagamento = novaVenda.DataRegistro;
                novaVenda.FormaPagamento = Enums.FormaPagamento.Dinheiro;
                novaVenda.DadosProdutos = "";
                novaVenda.StatusVenda = Enums.StatusVenda.Iniciada;
                novaVenda.ValorTotal = 0;
                //venda.ItensVenda = itensVenda;
                novaVenda.NFE = "";
                novaVenda.StatusPagamento = Enums.StatusPagamento.NaoPago;

                _vendaRepository.Cadastrar(novaVenda);
            }
            else
            {
                novaVenda.Id = ultimaVenda.Id + 1;
                novaVenda.ClienteId = 1;
                novaVenda.DataRegistro = PegaHoraBrasilia();
                novaVenda.DataPagamento = novaVenda.DataRegistro;
                novaVenda.FormaPagamento = Enums.FormaPagamento.Dinheiro;
                novaVenda.DadosProdutos = "";
                novaVenda.StatusVenda = Enums.StatusVenda.Iniciada;
                novaVenda.ValorTotal = 0;
                //venda.ItensVenda = itensVenda;
                novaVenda.NFE = "";
                novaVenda.StatusPagamento = Enums.StatusPagamento.NaoPago;

                _vendaRepository.Cadastrar(novaVenda);
            }

            ViewBag.IdVendaGerado = novaVenda.Id;
            ViewBag.Clientes = _clienteRepository.ObterTodosClientes().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
            ViewBag.Produtos = _produtoRepository.ObterTodosProdutos().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
            return View();
        }

        private StatusPagamento statusPagamento(Venda venda)
        {
            switch (venda.FormaPagamento)
            {
                case FormaPagamento.Prazo:
                    return StatusPagamento.NaoPago;
                case FormaPagamento.Dinheiro:
                case FormaPagamento.Cheque:
                case FormaPagamento.CartaoCrebito:
                case FormaPagamento.CartaoDebito:
                case FormaPagamento.Boleto:
                case FormaPagamento.Pix:
                    return StatusPagamento.Pago;
                default:
                    return StatusPagamento.Pago;

            }
        }

        private StatusVenda statusVenda(int status)
        {
            if (status == 1)
            {
                return StatusVenda.Finalizada;
            }
            else if (status == 2)
            {
                return StatusVenda.Suspensa;
            }
            else
            {
                return StatusVenda.Iniciada;
            }
        }

        [HttpPost]
        public IActionResult Cadastrar([FromForm] Models.Venda venda, int status)
        {            
            //erro aqui conversão de valor total
            var id = Convert.ToInt32(Request.Form["numVenda"]);
            var id_cliente = Convert.ToInt32(Request.Form["selectClienteVenda"]);
            var valorComVirgula = Request.Form["valorTotal"].ToString().Replace(".", ",");
            var custoComVirgula = Request.Form["custoTotal"].ToString().Replace(".", ",");
            var valor = Convert.ToDecimal(valorComVirgula);
            var custo = Convert.ToDecimal(custoComVirgula);

            if (ModelState.IsValid)
            {
                venda.Id = id;
                venda.ClienteId = id_cliente;
                venda.DataRegistro = PegaHoraBrasilia();
                venda.DataPagamento = venda.DataRegistro;
                venda.StatusPagamento = statusPagamento(venda);
                venda.StatusVenda = statusVenda(status);
                venda.ValorTotal = valor;
                venda.CustoTotal = custo;

                _vendaRepository.Atualizar(venda);

                TempData["MSG_S"] = "Venda finalizada com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Atualizar(int Id)
        {          
            
            Venda venda = _vendaRepository.ObterVenda(Id);

            ViewBag.Clientes = _clienteRepository.ObterTodosClientes().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
            ViewBag.Produtos = _produtoRepository.ObterTodosProdutos().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
            ViewBag.ItensVenda = _itemVendaRepository.ObterTodosItens(Id);

            return View(venda);

        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Venda venda, int status)
        {
            //erro aqui conversão de valor total
            var id = Convert.ToInt32(Request.Form["numVenda"]);
            var id_cliente = Convert.ToInt32(Request.Form["ClienteId"]);
            var valorComVirgula = Request.Form["valorTotal"].ToString().Replace(".", ",");
            var valor = Convert.ToDecimal(valorComVirgula);

            if (ModelState.IsValid)
            {
                venda.Id = id;
                venda.ClienteId = id_cliente;
                venda.DataRegistro = PegaHoraBrasilia();
                venda.DataPagamento = venda.DataRegistro;
                venda.StatusPagamento = statusPagamento(venda);
                venda.StatusVenda = statusVenda(status);
                venda.ValorTotal = valor;

                _vendaRepository.Atualizar(venda);

                TempData["MSG_S"] = "Venda finalizada com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Detalhar(int Id)
        {
            Venda venda = _vendaRepository.ObterVenda(Id);

            ViewBag.ItensVenda = _itemVendaRepository.ObterTodosItens(Id);

            return View(venda);

        }

        [HttpGet]
        public IActionResult Excluir(int Id)
        {
            List<ItemVenda> itens = _itemVendaRepository.ObterTodosItens(Id);
            foreach(ItemVenda item in itens)
            {
                _produtoRepository.IncrementaEstoque(item);
                _itemVendaRepository.Excluir(item);
            }
            _vendaRepository.Excluir(Id);

            TempData["MSG_S"] = Mensagem.MSG_S002;
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Receber(int Id)
        {
            Venda venda = _vendaRepository.ObterVenda(Id);
            venda.ItensVenda = _itemVendaRepository.ObterTodosItens(Id);

            return View(venda);
        }

        [HttpPost]
        public IActionResult Receber([FromForm] Models.Venda vendaRequest, int status)
        {
            int id = Convert.ToInt32(Request.Form["Id_venda"]);
            Venda venda = _vendaRepository.ObterVenda(id);

                venda.StatusPagamento = StatusPagamento.Pago;
                venda.StatusVenda = StatusVenda.Finalizada;
                venda.DataPagamento = PegaHoraBrasilia();
                venda.FormaPagamento = vendaRequest.FormaPagamento;
                _vendaRepository.Atualizar(venda);

                TempData["MSG_S"] = "Venda recebida com sucesso!";
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult unidadesProdutosVendidos(string dataInicio)
        {
            DateTime inicio = PegaDiaBrasilia();

            if (!String.IsNullOrEmpty(dataInicio))
            {
                inicio = Convert.ToDateTime(dataInicio);
            }

            /*
             *necessario obter primeira venda da data selecionada
             *para pegar a partir dos itens dessa venda
             *porque os itens venda não tem atributo data e hora
            */
            Venda venda = _vendaRepository.ObterPrimeiraVendaPorData(inicio);

            //cria uma lista vazia para receber os itens sem duplicidade
            List<ItemVenda> produtosVendidos = new List<ItemVenda>();
            
            if(venda != null) {
                //cria uma lista que vai receber todos os itens do banco (com duplicidade)
                List<ItemVenda> itensHoje = _itemVendaRepository.TodosItensPartindoDestaVenda(venda.Id);

                foreach (ItemVenda itemHoje in itensHoje)
                {
                    //verifica se na lista "produtosVendidos" já existe esse item
                    if (produtosVendidos.Exists(x => x.ProdutoId == itemHoje.ProdutoId))
                    {
                        //encontramos esse item 
                        ItemVenda item = produtosVendidos.Find(x => x.ProdutoId == itemHoje.ProdutoId);
                        //pegamos o indice
                        int indice = produtosVendidos.IndexOf(item);
                        //e somamos a qtde do item que veio do banco 
                        produtosVendidos[indice].Qtde += itemHoje.Qtde;

                    }
                    else
                    {
                        produtosVendidos.Add(itemHoje);
                    }

                }
                ViewBag.DataHoje = inicio.ToString("yyyy-MM-dd");
                //devolvemos a lista povoada para a view
                return View(produtosVendidos);
            }
            else
            {
                ViewBag.DataHoje = inicio.ToString("yyyy-MM-dd");
                return View(produtosVendidos);
            }
        }

        [HttpGet]
        public IActionResult VendasPeriodo(int periodo)
        {

            //return RedirectToAction(nameof(Index((1, 0, "", "", "")))) ;

            //return new RedirectToActionResult(Index(1, 0, "", "", ""), "Venda");


            return View();
        }

    }

}
