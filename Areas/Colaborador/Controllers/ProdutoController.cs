using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVendas.Libraries.Arquivo;
using SistemaVendas.Libraries.Filtro;
using SistemaVendas.Libraries.Lang;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace SistemaVendas.Areas.Colaborador.Controllers
{
    [Area("Colaborador")]
    [ColaboradorAutorizacao]
    public class ProdutoController : Controller
    {
        private IProdutoRepository _produtoRepository;
        private IFornecedorRepository _fornecedorRepository;
        private IImagemRepository _imagemRepository;
        private IHttpContextAccessor _context;

        public ProdutoController(IFornecedorRepository fornecedorRepository, IProdutoRepository produtoRepository, IImagemRepository imagemRepository, IHttpContextAccessor context)
        {
            _fornecedorRepository = fornecedorRepository;
            _produtoRepository = produtoRepository;
            _imagemRepository = imagemRepository;
            _context = context;
        }
        public IActionResult Index(int? pagina, string pesquisa)
        {
            var produtos = _produtoRepository.ObterTodosProdutos(pagina, pesquisa);
            return View(produtos);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            ViewBag.Fornecedores = _fornecedorRepository.ObterFornecedores().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Produto produto) {
            if (ModelState.IsValid)
            {
                 _produtoRepository.Cadastrar(produto);
                List<Imagem> ListaImagensDefinitiva = GerenciadorArquivo.MoverImagensProduto(new List<string>(Request.Form["imagem"]), produto.Id);
                _imagemRepository.CadastrarImagens(ListaImagensDefinitiva, produto.Id);

                TempData["MSG_S"] = Mensagem.MSG_S001;

                return RedirectToAction(nameof(Index));
            }
            else
            {
                produto.Imagens = new List<string>(Request.Form["imagem"]).Where(a=>a.Trim().Length > 0).Select(a => new Imagem() { Caminho = a }).ToList();
                
                return View(produto);
            }
        }

        [HttpGet]
        public IActionResult Atualizar(int id)
        {
            Produto produto = _produtoRepository.ObterProduto(id);
            ViewBag.Fornecedores = _fornecedorRepository.ObterFornecedores().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
            return View(produto);
        }

        //todo transformar metodo em suprir estoque
        [HttpGet]
        public IActionResult SuprirEstoque(int id)
        {
            Produto produto = _produtoRepository.ObterProduto(id);
            ViewBag.Estoque = produto.Estoque;
            return View(produto);
        }

        [HttpPost]
        public IActionResult SuprirEstoque(int id, decimal qtde)
        {
            Produto produto = _produtoRepository.ObterProduto(id);
            produto.Estoque += qtde;
            _produtoRepository.Atualizar(produto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Atualizar(Produto produto, int id)
        {
            if (ModelState.IsValid)
            {
                _produtoRepository.Atualizar(produto);

                //atualiza as imagens
                List<Imagem> ListaImagensDef = GerenciadorArquivo.MoverImagensProduto(new List<string>(Request.Form["imagem"]), produto.Id);
                _imagemRepository.ExcluirImagensDoProduto(produto.Id);
                _imagemRepository.CadastrarImagens(ListaImagensDef, produto.Id);

                TempData["MSG_S"] = Mensagem.MSG_S001;

                return RedirectToAction(nameof(Index));
            }
            else
            {
                produto.Imagens = new List<string>(Request.Form["imagem"]).Where(a => a.Trim().Length > 0).Select(a => new Imagem() { Caminho = a }).ToList();

                return View(produto);
            }
        }
        [HttpGet]
        [ValidateHttpReferer]
        public IActionResult Excluir(int id)
        {
            Produto produto = _produtoRepository.ObterProduto(id);
            GerenciadorArquivo.ExcluirImagensProduto(produto.Imagens.ToList());
            _imagemRepository.ExcluirImagensDoProduto(id);
            _produtoRepository.Excluir(id);

            TempData["MSG_S"] = Mensagem.MSG_S002;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult BuscaProdutoAjax(int id)
        {
            try
            {
                Produto produto = _produtoRepository.ObterProdutoAjax(id);

                string produtoJson = JsonConvert.SerializeObject(produto);
                return Ok(produtoJson);

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}