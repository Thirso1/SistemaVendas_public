using SistemaVendas.Database;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVendas.Repositories
{
    public class ItemVendaRepository : IItemVendaRepository
    {

        LojaVirtualContext _banco;

        public ItemVendaRepository(LojaVirtualContext banco)
        {
            _banco = banco;
        }

        public void Atualizar(ItemVenda itemVenda)
        {
            throw new NotImplementedException();
        }

        public void CadastrarItens(ItemVenda itensVenda)
        {          
            _banco.Add(itensVenda);
            _banco.SaveChanges();            
        }

        public void Excluir(ItemVenda item)
        {
            _banco.Remove(item);
            _banco.SaveChanges();
        }

        public void ExcluirTodos(int Id)
        {
            throw new NotImplementedException();
        }

        public ItemVenda ObterItemVenda(int Id)
        {
            throw new NotImplementedException();
        }

        public List<ItemVenda> ObterTodosItens(int idVenda)
        {
            return _banco.ItensVenda.Where(a => a.VendaId == idVenda).ToList();

        }

        public List<ItemVenda> TodosItensPartindoDestaVenda(int idVenda)
        {
            return _banco.ItensVenda.Where(a => a.VendaId >= idVenda).ToList();
        }

    }
}
