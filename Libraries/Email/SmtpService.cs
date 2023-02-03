using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;


namespace SistemaVendas.Libraries.Email
{
    public class SmtpService
    {
        private static IConfiguration _configuration;

        public SmtpService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static bool Send(string para, string paraTitulo, string assunto, string conteudo, List<string> attachments = null)
        {
            //string emailOrigem = _configuration.GetValue<string>("Email:Username");
            //string empresaOrigem = _configuration.GetValue<string>("Email:NomeEmpresa");

            //todo !!!URGENTE!!! mudar esses parametros para development.json 
            return Send("lojavirtual@estiloscountry.com.br", "Loja Estilos Country", para, paraTitulo, assunto, conteudo, attachments);
        }
        public static bool Send(string de, string deTitulo, string para, string paraTitulo, string assunto, string conteudo, List<string> attachments = null)
        {
            Stream memoryStream = null;
            try
            {
                string FromAddress = de;
                string FromAdressTitle = deTitulo;
                string ToAddress = para;
                string ToAdressTitle = paraTitulo;
                string Subject = assunto;
                string BodyContent = conteudo;
                //todo !!!URGENTE!!! mudar esses parametros para development.json 
                string SmtpServer = "bateaquihost.com.br";
                int SmtpPortNumber = 465;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(FromAdressTitle, FromAddress));
                foreach (var item in ToAddress?.Split(';'))
                {
                    if (!string.IsNullOrEmpty(item?.Trim()))
                        mimeMessage.To.Add(new MailboxAddress(ToAdressTitle, item?.ToLower(CultureInfo.InvariantCulture).Trim()));
                }
                mimeMessage.Subject = Subject;

                var multipart = new Multipart("mixed");
                multipart.Add(new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = BodyContent
                });

                if (attachments != null)
                {
                    foreach (var item in attachments)
                    {

                        if (item.StartsWith("http", StringComparison.InvariantCulture))
                        {
                            using var webclient = new System.Net.WebClient();
                            var downloadDataTaskAsync = webclient.DownloadDataTaskAsync(item);
                            downloadDataTaskAsync.Wait();
                            var bytes = downloadDataTaskAsync.Result;
                            memoryStream = new MemoryStream(bytes);
                        }
                        else
                        {
                            memoryStream = File.OpenRead(item);
                        }
                        var attachmentExtension = item.Split('.')[item.Split('.').Length - 1].Trim().ToLowerInvariant();
                        var attachment = new MimePart("application", attachmentExtension)
                        {
                            Content = new MimeContent(memoryStream, ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = item.Split('/').LastOrDefault().Split('\\').LastOrDefault(),
                        };
                        multipart.Add(attachment);
                    }
                }

                mimeMessage.Body = multipart;

                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, true);
                    //todo !!!URGENTE!!! mudar esses parametros para development.json 
                    client.Authenticate("lojavirtual@estiloscountry.com.br", "Lojacountry@2014");
                    client.SendAsync(mimeMessage).Wait();
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Dispose();
            }
        }
    }
}
