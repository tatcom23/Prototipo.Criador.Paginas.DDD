using Microsoft.EntityFrameworkCore;
using Redirect.Domain.Entities;
using Redirect.Domain.Repositories.Interfaces;
using Redirect.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redirect.Infrastructure.Repositories
{
    public class RedirecionamentoOrigemRepository : IRedirecionamentoOrigemRepository
    {
        private readonly AppDbContext _context;

        public RedirecionamentoOrigemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RedirecionamentoOrigem?> ObterPorIdAsync(int id)
        {
            return await _context.RedirecionamentoOrigens
                .Include(r => r.Destinos)
                .FirstOrDefaultAsync(r => r.Codigo == id);
        }

        public async Task<RedirecionamentoOrigem?> ObterPorUrlOrigemAsync(string urlOrigem)
        {
            return await _context.RedirecionamentoOrigens
                .FirstOrDefaultAsync(r => r.UrlOrigem.ToLower() == urlOrigem.ToLower() && r.Ativo);
        }

        public async Task<RedirecionamentoOrigem?> ObterComDestinosAsync(string urlOrigem)
        {
            return await _context.RedirecionamentoOrigens
                .Include(r => r.Destinos)
                .FirstOrDefaultAsync(r => r.UrlOrigem.ToLower() == urlOrigem.ToLower() && r.Ativo);
        }

        public async Task<IEnumerable<RedirecionamentoOrigem>> ObterTodosAsync()
        {
            return await _context.RedirecionamentoOrigens
                .Include(r => r.Destinos)
                .ToListAsync();
        }

        public async Task AdicionarAsync(RedirecionamentoOrigem redirecionamentoOrigem)
        {
            _context.RedirecionamentoOrigens.Add(redirecionamentoOrigem);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(RedirecionamentoOrigem redirecionamentoOrigem)
        {
            _context.RedirecionamentoOrigens.Update(redirecionamentoOrigem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var entity = await _context.RedirecionamentoOrigens.FindAsync(id);
            if (entity != null)
            {
                _context.RedirecionamentoOrigens.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
