using Microsoft.EntityFrameworkCore;
using Redirect.Domain.Entities;
using Redirect.Infrastructure.Data.Configurations;

namespace Redirect.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<RedirectURL> RedirectURLs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RedirectURLConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
