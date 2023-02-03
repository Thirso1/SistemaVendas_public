using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVendas.Database;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using Microsoft.Extensions.Configuration;
using X.PagedList;

namespace SistemaVendas.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private IConfiguration _conf;
        private LojaVirtualContext _banco;

        public ClienteRepository(LojaVirtualContext banco, IConfiguration conf)
        {
            _banco = banco;
            _conf = conf;
        }

        public void Atualizar(Cliente cliente)
        {
            _banco.Update(cliente);
            _banco.SaveChanges();
        }

        public void Cadastrar(Cliente cliente)
        {
            _banco.Add(cliente);
            _banco.SaveChanges();
        }

        public void Excluir(int Id)
        {
            Cliente cliente = ObterCliente(Id);
            _banco.Remove(cliente);
            _banco.SaveChanges();
        }

        public Cliente Login(string Email, string Senha)
        {
           // Cliente cliente = _banco.Clientes.Where(m => m.Email == Email && m.Senha == Senha).FirstOrDefault();
            return new Cliente { };
        }

        public Cliente ObterCliente(int Id)
        {
            return _banco.Clientes.Find(Id);
        }

        public Cliente ObterClientePorEmail(string email)
        {
            return _banco.Clientes.FirstOrDefault(a => a.Email == email);
        }

        public List<Cliente> ObterTodosClientes()
        {
            var bancoCliente = _banco.Clientes.AsQueryable().OrderBy(a => a.Nome);

            return bancoCliente.ToList();
        }
        public IPagedList<Cliente> ObterTodosClientes(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;

            var bancoCliente = _banco.Clientes.AsQueryable();
            if( !string.IsNullOrEmpty(pesquisa) )
            {
                bancoCliente = bancoCliente.Where(a => a.Nome.Contains(pesquisa.Trim()) || a.Email.Contains(pesquisa.Trim()));
            }

            return bancoCliente.ToPagedList<Cliente>(NumeroPagina, RegistroPorPagina);
        }

        public int QuantidadeTotalClientes()
        {
            return _banco.Clientes.Count();
        }
    }
}
