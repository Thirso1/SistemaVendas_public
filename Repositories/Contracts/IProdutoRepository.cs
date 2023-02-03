using SistemaVendas.Models;
using SistemaVendas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SistemaVendas.Repositories.Contracts
{
    public interface IProdutoRepository
    {
        //CRUD
        void Cadastrar(Produto produto);
        void Atualizar(Produto produto);
        public void AtualizarEstoque(ItemVenda item);
        public void IncrementaEstoque(ItemVenda item);
        void Excluir(int Id);
        Produto ObterProduto(int Id);
        public Produto ObterProdutoAjax(int Id);
        IPagedList<Produto> ObterTodosProdutos(int? pagina, string pesquisa);
        public List<Produto> ObterTodosProdutos();
        IPagedList<Produto> ObterTodosProdutos(int? pagina, string pesquisa, string ordenacao);
        int QuantidadeTotalProdutos();
        decimal ativosEmEstoque();
    }
}
