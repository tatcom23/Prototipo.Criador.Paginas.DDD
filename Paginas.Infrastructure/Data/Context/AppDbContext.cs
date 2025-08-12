using Microsoft.EntityFrameworkCore;
using Paginas.Domain.Entities;
using Paginas.Infrastructure.Data.Configurations;

namespace Paginas.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pagina> Paginas { get; set; }
        public DbSet<Botao> Botoes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaginaConfiguration());
            modelBuilder.ApplyConfiguration(new BotaoConfiguration());
        }
    }
}
