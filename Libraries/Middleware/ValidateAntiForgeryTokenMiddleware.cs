﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVendas.Libraries.Middleware
{
    public class ValidateAntiForgeryTokenMiddleware
    {
        private RequestDelegate _next;
        private IAntiforgery _antiforgery;

        public ValidateAntiForgeryTokenMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task Invoke(HttpContext context)
        {
            // var teste = context.Request.Headers["Accept"];
            var Cabecalho = context.Request.Headers["x-requested-with"];
            bool AJAX = (Cabecalho == "XMLHttpRequest") ? true : false;

            if (HttpMethods.IsPost(context.Request.Method) && !(context.Request.Form.Files.Count == 1 && AJAX))
            {
                await _antiforgery.ValidateRequestAsync(context);
            }

            await _next(context);
        }
    }
}
