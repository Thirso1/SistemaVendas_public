using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVendas.Database;
using SistemaVendas.Models;
using Newtonsoft.Json;
using SistemaVendas.Repositories;
using SistemaVendas.Repositories.Contracts;
using System.Threading;

namespace SistemaVendas.Areas.Colaborador.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemVendasController : ControllerBase
    {
        private IItemVendaRepository _itemVendaRepository;
        private IVendaRepository _vendaRepository;
        private IProdutoRepository _produtoRepository;

        public ItemVendasController(IItemVendaRepository itemVendaRepositoryory, IVendaRepository vendaRepository, IProdutoRepository produtoRepository)
        {
            _itemVendaRepository = itemVendaRepositoryory;
            _produtoRepository = produtoRepository;
            _vendaRepository = vendaRepository;

        }

        // GET: api/ItemVendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemVenda>>> GetItensVenda()
        {
            throw new NotImplementedException();

            //return await _context.ItensVenda.ToListAsync();
        }

        // GET: api/ItemVendas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemVenda>> GetItemVenda(int id)
        {
            throw new NotImplementedException();

            //var itemVenda = await _context.ItensVenda.FindAsync(id);

            //if (itemVenda == null)
            //{
            //    return NotFound();
            //}

            //return itemVenda;
        }

        [HttpPost]
        public void PutItensVenda([FromBody] List<ItemVenda> itens)
        {
            Thread.Sleep(1000);

            List<ItemVenda> itens_a_excluir = _itemVendaRepository.ObterTodosItens(itens[0].VendaId);

            foreach (ItemVenda itemExcluir in itens_a_excluir)
            {
                _itemVendaRepository.Excluir(itemExcluir);
            }

            foreach (ItemVenda item in itens)
            {
                _itemVendaRepository.CadastrarItens(item);
                _produtoRepository.AtualizarEstoque(item);
            }
        }

        [HttpPost("suspensos")]
        public void SuspendItensVenda([FromBody] List<ItemVenda> itens)
        {
            List<ItemVenda> itens_a_excluir = _itemVendaRepository.ObterTodosItens(itens[0].VendaId);
            foreach (ItemVenda itemExcluir in itens_a_excluir)
            {
                _itemVendaRepository.Excluir(itemExcluir);
            }

            Thread.Sleep(1000);

            foreach (ItemVenda item in itens)
            {
                _itemVendaRepository.CadastrarItens(item);
            }
        }

        // POST: api/ItemVendas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.


        // DELETE: api/ItemVendas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ItemVenda>> DeleteItemVenda(int id)
        {
            throw new NotImplementedException();

            //var itemVenda = await _context.ItensVenda.FindAsync(id);
            //if (itemVenda == null)
            //{
            //    return NotFound();
            //}

            //_context.ItensVenda.Remove(itemVenda);
            //await _context.SaveChangesAsync();

            //return itemVenda;
        }

        private bool ItemVendaExists(int id)
        {
            throw new NotImplementedException();

            //return _context.ItensVenda.Any(e => e.Id == id);
        }
    }
}

