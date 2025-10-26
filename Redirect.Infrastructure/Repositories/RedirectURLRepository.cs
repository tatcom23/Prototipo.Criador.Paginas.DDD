using Microsoft.EntityFrameworkCore;
using Redirect.Domain.Entities;
using Redirect.Domain.Repositories.Interfaces;
using Redirect.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redirect.Infrastructure.Repositories
{
    public class RedirectURLRepository : IRedirectURLRepository
    {
        private readonly AppDbContext _context;

        public RedirectURLRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RedirectURL?> ObterPorIdAsync(int id)
        {
            return await _context.RedirectURLs.FirstOrDefaultAsync(r => r.Codigo == id);
        }

        public async Task<RedirectURL?> ObterPorUrlAntigaAsync(string urlAntiga)
        {
            return await _context.RedirectURLs  // ✅ Corrigido: nome do DbSet
                .FirstOrDefaultAsync(r => r.UrlAntiga.ToLower() == urlAntiga.ToLower() && r.Ativo);
        }

        public async Task<IEnumerable<RedirectURL>> ObterTodosAsync()
        {
            return await _context.RedirectURLs.ToListAsync();  // ✅ Corrigido
        }

        public async Task AdicionarAsync(RedirectURL redirectURL)
        {
            _context.RedirectURLs.Add(redirectURL);  // ✅ Corrigido
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(RedirectURL redirectURL)
        {
            _context.RedirectURLs.Update(redirectURL);  // ✅ Corrigido
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var entity = await _context.RedirectURLs.FindAsync(id);  // ✅ Corrigido
            if (entity != null)
            {
                _context.RedirectURLs.Remove(entity);  // ✅ Corrigido
                await _context.SaveChangesAsync();
            }
        }
    }
}
