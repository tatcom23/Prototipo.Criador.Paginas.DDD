using Microsoft.EntityFrameworkCore;
using Redirect.Domain.Entities;
using Redirect.Infrastructure.Data.Configurations;

namespace Redirect.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<RedirecionamentoOrigem> RedirecionamentoOrigens { get; set; }
        public DbSet<RedirecionamentoDestino> RedirecionamentoDestinos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RedirecionamentoOrigemConfiguration());
            modelBuilder.ApplyConfiguration(new RedirecionamentoDestinoConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
