using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVendas.Libraries.Filtro;
using SistemaVendas.Libraries.Lang;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVendas.Areas.Colaborador.Controllers
{
    [Area("Colaborador")]
    [ColaboradorAutorizacao]
    public class FornecedorController : Controller
    {

        private IFornecedorRepository _fornecedorRepository;

        public FornecedorController(IFornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public IActionResult Index(int? pagina, string pesquisa)
        {
            var fornecedores = _fornecedorRepository.ObterFornecedores(pagina, pesquisa);

            return View(fornecedores);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar([FromForm] Models.Fornecedor fornecedor, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                fornecedor.Situacao = SistemaVendas.Models.Contants.SituacaoConstant.Ativo;
                _fornecedorRepository.Cadastrar(fornecedor);
                // _loginCliente.Login(cliente);

                TempData["MSG_S"] = "Cadastro realizado com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public IActionResult Atualizar(int Id)
        {
            Models.Fornecedor fornecedor = _fornecedorRepository.ObterFornecedor(Id);

            return View(fornecedor);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Fornecedor fornecedor, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                fornecedor.Situacao = SistemaVendas.Models.Contants.SituacaoConstant.Ativo;
                _fornecedorRepository.Atualizar(fornecedor);
                // _loginCliente.Login(cliente);

                TempData["MSG_S"] = "Cadastro realizado com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [ValidateHttpReferer]
        public IActionResult Excluir(int id)
        {
            _fornecedorRepository.Excluir(id);

            TempData["MSG_S"] = Mensagem.MSG_S002;

            return RedirectToAction(nameof(Index));
        }
    }
}
