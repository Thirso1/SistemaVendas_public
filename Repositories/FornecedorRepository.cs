//using AngleSharp;
using SistemaVendas.Database;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SistemaVendas.Repositories
{
    public class FornecedorRepository : IFornecedorRepository
    {
        LojaVirtualContext _banco;

        public FornecedorRepository(LojaVirtualContext banco)
        {
            _banco = banco;
        }

        public void Atualizar(Fornecedor fornecedor)
        {
            _banco.Fornecedores.Update(fornecedor);
            _banco.SaveChanges();
        }

        public void Cadastrar(Fornecedor fornecedor)
        {
            _banco.Fornecedores.Add(fornecedor);
            _banco.SaveChanges();
        }

        public void Excluir(int Id)
        {
            Fornecedor fornecedor = ObterFornecedor(Id);
            _banco.Fornecedores.Remove(fornecedor);
            _banco.SaveChanges();
        }

        public Fornecedor ObterFornecedor(int Id)
        {
            return _banco.Fornecedores.Find(Id);
        }

        public List<Fornecedor> ObterFornecedores()
        {
            return _banco.Fornecedores.AsQueryable().ToList();
        }

        public IPagedList<Fornecedor> ObterFornecedores(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = 10;

            int NumeroPagina = pagina ?? 1;

            var bancoVenda = _banco.Fornecedores.AsQueryable();
            //if (!string.IsNullOrEmpty(pesquisa))
            //{
            //    bancoVenda = bancoVenda.Where(a => a.Cliente.Nome.Contains(pesquisa.Trim()));
            //}

            return bancoVenda.ToPagedList<Fornecedor>(NumeroPagina, RegistroPorPagina);
        }

    }
}
