using Microsoft.EntityFrameworkCore;
using Paginas.Infrastructure.Data.Context;
using System;

namespace Paginas.Infrastructure.Tests.TestHelpers
{
    public static class InMemoryContextFactory
    {
        // Nota: usamos 'string dbName = null' para compatibilidade com configurações que não
        // têm nullable context habilitado no projeto de testes.
        public static AppDbContext CreateContext(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
