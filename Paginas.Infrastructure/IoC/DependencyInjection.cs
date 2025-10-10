using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paginas.Infrastructure.Data.Context;
using Paginas.Domain.Repositories.Interfaces;
using Paginas.Infrastructure.Repositories;
using Paginas.Application.Services.Interfaces;
using Paginas.Application.Services;

namespace Paginas.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructurePaginas(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura o DbContext com a connection string correta
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(true); // bom para dev, desative em produção
            });

            // Repositórios
            services.AddScoped<IPaginaRepository, PaginaRepository>();
            services.AddScoped<IBotaoRepository, BotaoRepository>();
            services.AddScoped<ICarrosselRepository, CarrosselRepository>();
            services.AddScoped<ICarrosselImagemRepository, CarrosselImagemRepository>();

            // Serviços de aplicação
            services.AddScoped<IPaginaService, PaginaService>();
            services.AddScoped<IBotaoService, BotaoService>();
            services.AddScoped<ICarrosselService, CarrosselService>();
            services.AddScoped<ICarrosselImagemService, CarrosselImagemService>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}
