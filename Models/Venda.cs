using SistemaVendas.Enums;
using SistemaVendas.Libraries.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVendas.Models
{
    public class Venda
    {
        //PK
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal CustoTotal { get; set; }
        public string DadosProdutos { get; set; } //ProdutoItem - JSON

        public DateTime DataRegistro { get; set; }
        public DateTime DataPagamento { get; set; }
        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        public FormaPagamento FormaPagamento { get; set; } 
        public StatusVenda StatusVenda { get; set; }
        public StatusPagamento StatusPagamento { get; set; }

        //URL - Com Site da Receita - Nota Fiscal
        public string NFE { get; set; }

        [ForeignKey("Cliente")]
        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("VendaId")]
        public virtual ICollection<ItemVenda> ItensVenda { get; set; }

    }
}
