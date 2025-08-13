using Microsoft.EntityFrameworkCore;
using Paginas.Infrastructure.Data.Context;

public static class DbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(@"Server=SEPLAN-0372\SQLTATI;Database=PaginaIntrodutoria;Trusted_Connection=True;")
            .Options;

        var context = new AppDbContext(options);

        // Opcional: aplicar migrations automaticamente
        context.Database.Migrate();

        return context;
    }
}
