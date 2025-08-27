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
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")); // Corrigido
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(true);
            });

            // Repositórios
            services.AddScoped<IPaginaRepository, PaginaRepository>();
            services.AddScoped<IBotaoRepository, BotaoRepository>();

            // Serviços de aplicação
            services.AddScoped<IPaginaService, PaginaService>();
            services.AddScoped<IBotaoService, BotaoService>();

            return services;
        }
    }
}
