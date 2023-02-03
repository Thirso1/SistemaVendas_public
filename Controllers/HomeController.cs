using SistemaVendas.Libraries.Email;
using Microsoft.AspNetCore.Mvc;
using SistemaVendas.Repositories.Contracts;
using SistemaVendas.Libraries.Login;
using Microsoft.Extensions.Logging;

namespace SistemaVendas.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction( "Login", "Home", new { area = "Colaborador" });
        }        
    }
}