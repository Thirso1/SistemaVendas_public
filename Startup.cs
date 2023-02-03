using SistemaVendas.Database;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SistemaVendas.Repositories;
using SistemaVendas.Repositories.Contracts;
using SistemaVendas.Libraries.Sessao;
using SistemaVendas.Libraries.Login;
using System.Net.Mail;
using System.Net;
using SistemaVendas.Libraries.Email;
using SistemaVendas.Libraries.Middleware;
using AutoMapper;
using SistemaVendas.Libraries.AutoMapper;
//using Coravel;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;

namespace SistemaVendas
{
    public class Startup
    {
        //private object serverVersion;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            /*
             * API - Logging
             */
            services.AddLogging();
            /*
             * AutoMapper
             */
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
            services.AddAutoMapper(config => config.AddProfile<MappingProfile>());
#pragma warning restore CS0618 // O tipo ou membro é obsoleto

            /*
             * Padrão Repository
             */
            services.AddHttpContextAccessor();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IImagemRepository, ImagemRepository>();
            services.AddScoped<IVendaRepository, VendaRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IItemVendaRepository, ItemVendaRepository>();

            /*
             * SMTP
             */
            //services.AddScoped<SmtpClient>(options =>
            //{
            //    SmtpClient smtp = new SmtpClient()
            //    {
            //        Host = Configuration.GetValue<string>("Email:ServerSMTP"),
            //        Port = Configuration.GetValue<int>("Email:ServerPort"),
            //        UseDefaultCredentials = false,
            //        Credentials = new NetworkCredential(Configuration.GetValue<string>("Email:Username"), Configuration.GetValue<string>("Email:Password")),
            //        EnableSsl = true
            //    };

            //    return smtp;
            //});

            //services.AddScoped<GerenciarEmail>();
            services.AddScoped<SistemaVendas.Libraries.Cookie.Cookie>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IISOptions>(o =>
            {
                o.ForwardClientCertificate = false;
            });

            /*
             * Session - Configuração
             */
            services.AddMemoryCache(); //Guardar os dados na memória            

            services.AddScoped<Sessao>();
            services.AddScoped<SistemaVendas.Libraries.Cookie.Cookie>();
            services.AddScoped<LoginCliente>();
            services.AddScoped<LoginColaborador>();
            //services.AddScoped<GerenciarCielo>();

            services.AddMvc(options =>
            {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "O campo deve ser preenchido!");
            })
            .AddSessionStateTempDataProvider();

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });

            //string connection = "Server=tcp:loja-virtual-servidor.database.windows.net,1433;Initial Catalog=loja-virtual-banco;Persist Security Info=False;User ID=thirso;Password=W5xZ33W4;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
            //services.AddDbContext<LojaVirtualContext>(options => options.UseSqlServer(connection));

            //jair
            var connectionString = "server=mysql.bateaquihost.com.br;user=adilson2110260116_jair; password=mat89148029;database=adilson2110260116_jair";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
            //multiplot
            //var connectionString = "server=mysql.bateaquihost.com.br;user=adilson2110260116_multplot; password=mat89148029;database=adilson2110260116_multplot";
            //var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
            //localhost
            //var connectionString = "server=localhost;port=3308;user=root; password=wiUsRU0cMo6ZVsip;database=adilson2110260116_jair";
            //var serverVersion = new MySqlServerVersion(new Version(7, 5, 23));

            //services.AddDbContext<LojaVirtualContext>(
            services.AddDbContextPool<LojaVirtualContext>(
            options => options
            .UseMySql(connectionString, serverVersion)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
        );


            //services.AddScheduler();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
                //app.UseExceptionHandler("/Error/Error500");
                //The default HSTS value is 30 days.You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
                //app.UseExceptionHandler("/Error/Error500");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseSession();
            //app.UseMiddleware<ValidateAntiForgeryTokenMiddleware>();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                  );
                endpoint.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller=Home}/{action=Index}/{id?}");
            });

            /*
             Scheduler - Coravel
             */
            //app.ApplicationServices.UseScheduler(scheduler =>
            //{
            //    scheduler.Schedule<PedidoPagamentoSituacaoJob>().EveryThirtySeconds();
            //    scheduler.Schedule<PedidoEntregueJob>().EveryThirtySeconds();
            //    scheduler.Schedule<PedidoFinalizadoJob>().EveryThirtySeconds();
            //    scheduler.Schedule<PedidoDevolverEntregueJob>().EveryThirtySeconds();
            //});


        }
    }
}
