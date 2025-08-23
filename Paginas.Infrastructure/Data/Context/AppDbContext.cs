using Microsoft.EntityFrameworkCore;
using Paginas.Domain.Entities;
using Paginas.Infrastructure.Data.Configurations;

namespace Paginas.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pagina> Paginas { get; set; }
        public DbSet<Botao> Botoes { get; set; }

        // Construtor padrão
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Construtor opcional com string de conexão
        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Se não estiver configurado externamente, usar SQL Server com string de conexão padrão
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=PaginaIntrodutoria;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaginaConfiguration());
            modelBuilder.ApplyConfiguration(new BotaoConfiguration());
        }
    }
}
