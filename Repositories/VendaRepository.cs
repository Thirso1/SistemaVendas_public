using SistemaVendas.Database;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SistemaVendas.Enums;
using System.Globalization;

namespace SistemaVendas.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        IConfiguration _conf;
        LojaVirtualContext _banco;
        public DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));


        public VendaRepository(LojaVirtualContext banco, IConfiguration configuration)
        {
            _banco = banco;
            _conf = configuration;
        }

        public void Atualizar(Venda venda)
        {
            _banco.Update(venda);
            _banco.SaveChanges();
        }

        public void Cadastrar(Venda venda)
        {
            _banco.Add(venda);
            _banco.SaveChanges();
        }

        public void Excluir(int Id)
        {
            Venda venda = ObterVenda(Id);
            _banco.Remove(venda);
            _banco.SaveChanges();
        }

        public Venda RetornaUltimaVenda()
        {
            return (from v in _banco.Vendas orderby v.Id descending select v).FirstOrDefault();

        }

        public Venda ObterVenda(int Id)
        {
            return _banco.Vendas.Include("Cliente").First(p => p.Id == Id);
        }

        public IPagedList<Venda> ObterTodasVendas(int? pagina)
        {
            int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;

            var bancoVenda = _banco.Vendas.Include(a => a.Cliente).OrderByDescending(a => a.DataRegistro).ToList();
            //esse if remove a ultima venda se ela for apenas "iniciada"
            if(bancoVenda.First().StatusVenda == Enums.StatusVenda.Iniciada)
            {
                bancoVenda.Remove(bancoVenda.First());
            }

            return bancoVenda.ToPagedList<Venda>(NumeroPagina, RegistroPorPagina);
        }

        public IPagedList<Venda> ObterVendas(int? pagina, DateTime dataInicio, DateTime dataFim)
        {
            int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");
            int NumeroPagina = pagina ?? 1;

            var bancoVenda = _banco.Vendas.Where(p => p.DataRegistro >= dataInicio && p.DataRegistro <= dataFim).Include(a => a.Cliente).OrderByDescending(a => a.DataPagamento);

            return bancoVenda.ToPagedList<Venda>(NumeroPagina, RegistroPorPagina);
        }

        public IPagedList<Venda> ObterVendas(int? pagina, int ClienteId, DateTime dataInicio, DateTime dataFim)
        {
            int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");
            int NumeroPagina = pagina ?? 1;

            var bancoVenda = _banco.Vendas.Where(p => p.DataRegistro >= dataInicio && p.DataRegistro <= dataFim).Include(a => a.Cliente).Where(a => a.Cliente.Id == ClienteId).OrderByDescending(a => a.DataPagamento);

            return bancoVenda.ToPagedList<Venda>(NumeroPagina, RegistroPorPagina);
        }

        public IPagedList<Venda> ObterVendas(int? pagina, StatusPagamento status, DateTime dataInicio, DateTime dataFim)
        {
            int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");
            int NumeroPagina = pagina ?? 1;

            var bancoVenda = _banco.Vendas.Where(p => p.DataRegistro >= dataInicio && p.DataRegistro <= dataFim && p.StatusPagamento == status).Include(a => a.Cliente).OrderByDescending(a => a.DataPagamento);

            return bancoVenda.ToPagedList<Venda>(NumeroPagina, RegistroPorPagina);
        }

        public IPagedList<Venda> ObterVendas(int? pagina, int ClienteId, StatusPagamento status, DateTime dataInicio, DateTime dataFim)
        {
            int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");
            int NumeroPagina = pagina ?? 1;

            var bancoVenda = _banco.Vendas.Where(p => p.DataRegistro >= dataInicio && p.DataRegistro <= dataFim && p.StatusPagamento == status && p.ClienteId == ClienteId).Include(a => a.Cliente).OrderByDescending(a => a.DataPagamento);

            return bancoVenda.ToPagedList<Venda>(NumeroPagina, RegistroPorPagina);
        }




        public decimal VendasHoje()
        {
            //total das vendas que foram feitas hoje e recebidas hoje
            var total = _banco.Vendas.Where(t => t.DataPagamento >= PegaHoraBrasilia() && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);
            decimal test = Convert.ToDecimal(total);

            return Convert.ToDecimal(total);
        }

        public DateTime Semana()
        {
            DateTime hoje = PegaHoraBrasilia();
            int numDiaSemana = Convert.ToInt32(hoje.DayOfWeek);
            //decrementa -1 , cliente pediu para a semana começar na segunda-feira
            numDiaSemana -= 1;
            return PegaHoraBrasilia().AddDays(-numDiaSemana);
        }
        public decimal VendasSemana()
        {
            DateTime semana = Semana();
            return _banco.Vendas.Where(t => t.DataPagamento >= semana && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);
        }

        public DateTime Mes()
        {
            DateTime hoje = PegaHoraBrasilia();
            int numDiaMes = Convert.ToInt32(hoje.Day);
            //decrementa -1 , cliente pediu para a semana começar na segunda-feira
            return DateTime.Today.AddDays(-numDiaMes);
        }
        public decimal VendasMes()
        {
            DateTime mes = Mes();
            var total = _banco.Vendas.Where(t => t.DataPagamento >= mes && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);
            return Convert.ToDecimal(total);
        }

        public decimal LucroHoje()
        {

            var custo = _banco.Vendas.Where(t => t.DataPagamento >= PegaHoraBrasilia() && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);
            var total = _banco.Vendas.Where(t => t.DataPagamento >= PegaHoraBrasilia() && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);

            return total - custo;

        }

        public decimal LucroSemana()
        {
            DateTime semana = Semana();

            var custo = _banco.Vendas.Where(t => t.DataPagamento >= semana && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);
            var total = _banco.Vendas.Where(t => t.DataPagamento >= semana && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);

            return total - custo;
        }

        public decimal LucroMes()
        {
            DateTime mes = Mes();
            var custo = _banco.Vendas.Where(t => t.DataPagamento >= mes && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);
            var total = _banco.Vendas.Where(t => t.DataPagamento >= mes && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);
            return total - custo;
        }

        public decimal TotalVendasPrazo()
        {
            var total = _banco.Vendas.Where(t => t.StatusPagamento == StatusPagamento.NaoPago).Sum(i => i.ValorTotal);
            return total;
        }

        public Venda ObterPrimeiraVendaPorData(DateTime inicio)
        {
            DateTime fim = inicio.AddDays(1);
            Venda venda = (from v in _banco.Vendas.Where(v => v.DataRegistro >= inicio && v.DataRegistro < fim) orderby v.DataRegistro ascending select v).FirstOrDefault();
            return venda;
        }

        public decimal CustoHoje()
        {
            var custo = _banco.Vendas.Where(t => t.DataPagamento >= PegaHoraBrasilia() && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);

            return custo;
        }

        public decimal CustoSemana()
        {
            DateTime semana = Semana();

            var custo = _banco.Vendas.Where(t => t.DataPagamento >= semana && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);

            return custo;
        }

        public decimal CustoMes()
        {
            DateTime mes = DateTime.Today.AddDays(-(PegaHoraBrasilia().Day - 1));
            var custo = _banco.Vendas.Where(t => t.DataPagamento >= mes && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);
            return custo;
        }

        public decimal TotalPorData(DateTime data)
        {
            var total = _banco.Vendas.Where(t => t.DataPagamento >= data && t.DataPagamento <= data.AddDays(1) && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);
            decimal test = Convert.ToDecimal(total);

            return Convert.ToDecimal(total);
        }

        public decimal LucroPorData(DateTime data)
        {
            var custo = _banco.Vendas.Where(t => t.DataPagamento >= data && t.DataPagamento <= data.AddDays(1) && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);
            var total = _banco.Vendas.Where(t => t.DataPagamento >= data && t.DataPagamento <= data.AddDays(1) && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.ValorTotal);

            return total - custo;
        }

        public decimal CustoPorData(DateTime data)
        {
            var custo = _banco.Vendas.Where(t => t.DataPagamento >= data && t.DataPagamento <= data.AddDays(1) && t.StatusPagamento == StatusPagamento.Pago).Sum(i => i.CustoTotal);

            return custo;
        }
    }
}
