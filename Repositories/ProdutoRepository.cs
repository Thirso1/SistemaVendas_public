using SistemaVendas.Database;
using SistemaVendas.Libraries.Json.Resolver;
using SistemaVendas.Models;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using X.PagedList;

namespace SistemaVendas.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        IConfiguration _conf;
        LojaVirtualContext _banco;

        public ProdutoRepository(LojaVirtualContext banco, IConfiguration configuration)
        {
            _banco = banco;
            _conf = configuration;
        }

        public void Atualizar(Produto produto)
        {
            _banco.Update(produto);
            _banco.SaveChanges();
        }  
        public void AtualizarEstoque(ItemVenda item)
        {
            Produto produto = ObterProduto(item.ProdutoId);
            produto.Estoque -= item.Qtde;

            _banco.Update(produto);
            _banco.SaveChanges();
        }
        public void IncrementaEstoque(ItemVenda item)
        {
            Produto produto = ObterProduto(item.ProdutoId);
            produto.Estoque += item.Qtde;

            _banco.Update(produto);
            _banco.SaveChanges();
        }

        public void Cadastrar(Produto produto)
        {
            _banco.Add(produto);
            _banco.SaveChanges();
        }


        public void Excluir(int Id)
        {
            Produto produto = ObterProduto(Id);
            _banco.Remove(produto);
            _banco.SaveChanges();
        }

        public Produto ObterProduto(int Id)
        {
            Produto produto = _banco.Produtos.Include(a => a.Imagens).OrderBy(a=>a.Nome).Where(a=>a.Id == Id).FirstOrDefault();
            return produto;

        }
        public Produto ObterProdutoAjax(int Id)
        {
            return _banco.Produtos.Find(Id);
        }

        public IPagedList<Produto> ObterTodosProdutos(int? pagina, string pesquisa)
        {
            return ObterTodosProdutos(pagina, pesquisa, "A");
        }
        public List<Produto> ObterTodosProdutos()
        {
            return _banco.Produtos.AsQueryable().OrderBy(a => a.Nome).ToList();
        }

        public IPagedList<Produto> ObterTodosProdutos(int? pagina, string pesquisa, string ordenacao)
        {
            int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;

            var bancoProduto = _banco.Produtos.AsQueryable();
            if (!string.IsNullOrEmpty(pesquisa))
            {
                bancoProduto = bancoProduto.Where(a => a.Nome.Contains(pesquisa.Trim()));
            }
            if(ordenacao == "A")
            {
                bancoProduto = bancoProduto.OrderBy(a => a.Nome);
            }
            if (ordenacao == "ME")
            {
                bancoProduto = bancoProduto.OrderBy(a => a.ValorVarejo);
            }
            if (ordenacao == "MA")
            {
                bancoProduto = bancoProduto.OrderByDescending(a => a.ValorVarejo);
            }

            return bancoProduto.Include(a => a.Imagens).ToPagedList<Produto>(NumeroPagina, RegistroPorPagina);
        }

        public decimal ativosEmEstoque()
        {
            decimal valor = 0;
            List<Produto> produtos = ObterTodosProdutos();
            foreach(Produto p in produtos)
            {
                valor +=  p.ValorCusto * p.Estoque;
            }
            return valor;
        }

        public int QuantidadeTotalProdutos()
        {
            return _banco.Produtos.Count();
        }
    }
}
