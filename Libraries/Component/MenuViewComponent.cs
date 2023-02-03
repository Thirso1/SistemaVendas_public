using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVendas.Libraries.Component
{
    public class MenuViewComponent : ViewComponent
    {
        public MenuViewComponent()
        {


#pragma warning disable CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
             async Task<IViewComponentResult> InvokeAsync()
#pragma warning restore CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
            {

                return View();
            }
        }
    }
}
