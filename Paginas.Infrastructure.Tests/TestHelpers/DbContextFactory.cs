using System;
using Microsoft.EntityFrameworkCore;
using Paginas.Infrastructure.Data.Context;

public static class InMemoryContextFactory
{
    public static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // banco único por teste
            .Options;

        var context = new AppDbContext(options);

        // Cria todas as tabelas do modelo atual
        context.Database.EnsureCreated();

        return context;
    }
}
