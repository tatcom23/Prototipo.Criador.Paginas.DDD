using Microsoft.EntityFrameworkCore;
using Paginas.Infrastructure.Data.Context;

namespace Paginas.Infrastructure.Tests.TestHelpers
{
    public static class InMemoryContextFactory
    {
        public static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("PaginasTestDb")
                .Options;

            return new AppDbContext(options);
        }
    }
}
