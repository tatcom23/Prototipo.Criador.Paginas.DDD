using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redirect.Domain.Repositories.Interfaces;
using Redirect.Infrastructure.Repositories;
using Redirect.Infrastructure.Data.Context;
using Redirect.Application.Services.Interfaces;
using Redirect.Application.Services;

namespace Redirect.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureRedirect(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura o DbContext com a connection string correta
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(true); // bom para dev, desative em produção
            });

            // Repositórios
            services.AddScoped<IRedirectURLRepository, RedirectURLRepository>();
            
            // Serviços de aplicação
            services.AddScoped<IRedirectURLService, RedirectURLService>();

            return services;
        }
    }
}
