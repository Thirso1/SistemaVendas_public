using SistemaVendas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SistemaVendas.Repositories.Contracts
{
    public interface IFornecedorRepository
    {
        void Cadastrar(Fornecedor fornecedor);
        void Atualizar(Fornecedor fornecedor);
        void Excluir(int Id);
        Fornecedor ObterFornecedor(int Id);
        List<Fornecedor> ObterFornecedores();
        IPagedList<Fornecedor> ObterFornecedores(int? pagina, string pesquisa);

    }
}
