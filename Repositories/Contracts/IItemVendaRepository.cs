using SistemaVendas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SistemaVendas.Repositories.Contracts
{
    public interface IItemVendaRepository
    {
        void CadastrarItens(ItemVenda itensVenda);
        void Atualizar(ItemVenda itemVenda);
        void Excluir(ItemVenda item);
        void ExcluirTodos(int Id);
        ItemVenda ObterItemVenda(int Id);
        public List<ItemVenda> ObterTodosItens(int idVenda);
        public List<ItemVenda> TodosItensPartindoDestaVenda(int idVenda);
    }
}

