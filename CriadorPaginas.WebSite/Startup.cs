using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paginas.Infrastructure.IoC;
using Redirect.Infrastructure.IoC;
using Redirect.API.Middleware;

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
            services.AddInfrastructurePaginas(Configuration);
            services.AddInfrastructureRedirect(Configuration);
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

            app.UseMiddleware<RedirecionamentoMiddleware>();

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
