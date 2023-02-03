using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVendas.Areas.Colaborador.Controllers
{
    public class VendaFisicaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
