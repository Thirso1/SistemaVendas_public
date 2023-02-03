using SistemaVendas.Libraries.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SistemaVendas.Models
{
    public class Produto
    {
        //PK
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
                
        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Display(Name = "Preço Custo")]
        public decimal ValorCusto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Display(Name = "Preço Varejo")]
        public decimal ValorVarejo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Display(Name = "Preço Atacado")]
        public decimal ValorAtacado { get; set; }

        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Display(Name = "Desconto")]
        public decimal Desconto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Display(Name = "Estoque Atual")]
        public decimal Estoque { get; set; }

        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Display(Name = "Estoque Mínimo")]
        public decimal EstoqueMin { get; set; }

        [Display(Name = "Estoque Máximo")]
        public decimal EstoqueMax { get; set; }


        //Banco de dados - Relacionamento entre Tabela
        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Display(Name = "Fornecedor")]
        [JsonIgnore]
        public int FornecedorId { get; set; }

        //POO - Associações entre Objetos
        [ForeignKey("FornecedorId")]
        [JsonIgnore]
        public virtual Fornecedor Fornecedor { get; set; }
        [JsonIgnore]
        public virtual ICollection<Imagem> Imagens { get; set; }
        
    }
}
