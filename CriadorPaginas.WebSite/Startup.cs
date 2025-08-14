using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paginas.Infrastructure.Data.Context;        
using Paginas.Domain.Repositories.Interfaces;                 
using Paginas.Infrastructure.Repositories;          
using Paginas.Application.Services.Interfaces;      
using Paginas.Application.Services;                 

namespace CriadorPaginas.WebSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 1) Registros no container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // DbContext (SQL Server) -> usa a connection string "DefaultConnection" do appsettings.json
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Repositórios
            services.AddScoped<IPaginaRepository, PaginaRepository>();
            services.AddScoped<IBotaoRepository, BotaoRepository>();

            // Serviços de aplicação
            services.AddScoped<IPaginaService, PaginaService>();
            services.AddScoped<IBotaoService, BotaoService>();

            // AutoMapper (se você usar perfis no Application/Infrastructure)
            // services.AddAutoMapper(typeof(Startup));
        }

        // 2) Pipeline HTTP
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Pagina}/{action=Index}/{id?}");
            });

        }
    }
}
