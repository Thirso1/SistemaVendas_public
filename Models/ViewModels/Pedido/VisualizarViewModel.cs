using SistemaVendas.Libraries.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVendas.Models.ViewModels.Pedido
{
    public class VisualizarViewModel
    {      

        [Display(Name = "Motivo da rejeição")]
        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        public string DevolucaoMotivoRejeicao { get; set; }
    }
}
