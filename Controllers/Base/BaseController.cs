using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SistemaVendas.Libraries.Login;
using SistemaVendas.Libraries.Seguranca;
using SistemaVendas.Models;
using SistemaVendas.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SistemaVendas.Controllers.Base
{
    public class BaseController : Controller
    {
        protected IProdutoRepository _produtoRepository;
        protected IMapper _mapper;
        protected LoginCliente _loginCliente;

        public BaseController(LoginCliente loginCliente, IProdutoRepository produtoRepository, IMapper mapper)
        {
            _produtoRepository = produtoRepository;
            _mapper = mapper;
            _loginCliente = loginCliente;
        }


        protected string GerarHash(object obj)
        {
            return StringMD5.MD5Hash(JsonConvert.SerializeObject(obj));
        }
    }
}