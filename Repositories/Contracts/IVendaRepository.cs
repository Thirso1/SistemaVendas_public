using SistemaVendas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SistemaVendas.Repositories.Contracts
{
    public interface IVendaRepository
    {
        void Cadastrar(Venda venda);
        void Atualizar(Venda venda);
        void Excluir(int Id);
        Venda ObterVenda(int Id);
        public IPagedList<Venda> ObterTodasVendas(int? pagina);
       
        Venda RetornaUltimaVenda();

        Venda ObterPrimeiraVendaPorData(DateTime data);
        //todas
        public IPagedList<Venda> ObterVendas(int? pagina, DateTime dataInicio, DateTime dataFim);
        //por cliente e data
        public IPagedList<Venda> ObterVendas(int? pagina, int ClienteId, DateTime dataInicio, DateTime dataFim);
        //por status e data
        public IPagedList<Venda> ObterVendas(int? pagina, Enums.StatusPagamento status, DateTime dataInicio, DateTime dataFim);
        //por cliente, status e data
        public IPagedList<Venda> ObterVendas(int? pagina, int ClienteId, Enums.StatusPagamento status, DateTime dataInicio, DateTime dataFim);

        Decimal VendasHoje();
        public DateTime Semana();
        Decimal VendasSemana();
        Decimal VendasMes();
        Decimal LucroHoje();
        Decimal LucroSemana();
        Decimal LucroMes();
        Decimal CustoHoje();
        Decimal CustoSemana();
        Decimal CustoMes();
        Decimal TotalVendasPrazo();
        Decimal TotalPorData(DateTime data);
        Decimal LucroPorData(DateTime data);
        Decimal CustoPorData(DateTime data);



    }
}
