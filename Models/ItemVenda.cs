using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVendas.Models
{
    public class ItemVenda
    {
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nome { get; set; }

        public decimal ValorUnitario { get; set; }
        public decimal Desconto { get; set; }
        public decimal Qtde { get; set; }
        public decimal TotalItem { get; set; }

        [ForeignKey("Venda")]
        public int VendaId { get; set; }

        [ForeignKey("Produto")]
        public int ProdutoId { get; set; }
    }
}
