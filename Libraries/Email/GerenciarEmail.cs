using SistemaVendas.Libraries.Seguranca;
using SistemaVendas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SistemaVendas.Libraries.Email
{
    public class GerenciarEmail
    {
        private SmtpClient _smtp;
        private IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;


        public GerenciarEmail(SmtpClient smtp, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _smtp = smtp;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }



        public void EnviarSenhaParaColaboradorPorEmail(Colaborador colaborador)
        {
            string corpoMsg = string.Format("<h2>Colaborador - LojaVirtual</h2>" +
                "Sua senha é:" +
                "<h3>{0}</h3>", colaborador.Senha);


            /*
             * MailMessage -> Construir a mensagem
             */
            //MailMessage mensagem = new MailMessage();
            //mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            //mensagem.To.Add(colaborador.Email);
            //mensagem.Subject = "Colaborador - LojaVirtual - Senha do colaborador - " + colaborador.Nome;
            //mensagem.Body = corpoMsg;
            //mensagem.IsBodyHtml = true;

            //Enviar Mensagem via SMTP
            //_smtp.Send(mensagem);
            //_gerenciarEmail.EnviarContatoPorEmail(contato);

            SmtpService.Send(para: colaborador.Email, paraTitulo: "Senha do Colaborador.", assunto: "Colaborador - LojaVirtual - Senha do colaborador - " + colaborador.Nome, conteudo: corpoMsg + "<br />");

        }

    

        public void EnviarLinkResetarSenha(dynamic usuario, string idCrip)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string url = "";
            if (usuario.GetType() == typeof(Cliente))
            {
                url = $"{request.Scheme}://{request.Host}/Cliente/Home/CriarSenha/{idCrip}";
            }
            else
            {
                url = $"{request.Scheme}://{request.Host}/Colaborador/Home/CriarSenha/{idCrip}";
            }
            

            string corpoMsg = string.Format(
                "<h2>Criar nova Senha para {1}({2})</h2>" +
                "Clique no link abaixo para criar uma nova senha!<br />" +
                "<a href='{0}' target='_blank'>{0}</a>",
                url,
                usuario.Nome,
                usuario.Email
            );


            /*
             * MailMessage -> Construir a mensagem
             */
            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            mensagem.To.Add(usuario.Email);
            mensagem.Subject = "LojaVirtual - Criar nova senha - " + usuario.Nome;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            //Enviar Mensagem via SMTP
            _smtp.Send(mensagem);
        }
    }
}
