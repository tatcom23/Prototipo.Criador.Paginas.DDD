using Microsoft.EntityFrameworkCore;
using Redirect.Domain.Entities;
using Redirect.Domain.Repositories.Interfaces;
using Redirect.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redirect.Infrastructure.Repositories
{
    public class RedirecionamentoDestinoRepository : IRedirecionamentoDestinoRepository
    {
        private readonly AppDbContext _context;

        public RedirecionamentoDestinoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RedirecionamentoDestino?> ObterPorIdAsync(int id)
        {
            return await _context.RedirecionamentoDestinos
                .Include(d => d.RedirecionamentoOrigem)
                .FirstOrDefaultAsync(d => d.Codigo == id);
        }

        public async Task<IEnumerable<RedirecionamentoDestino>> ObterPorOrigemAsync(int origemId)
        {
            return await _context.RedirecionamentoDestinos
                .Where(d => d.RedirecionamentoOrigemId == origemId && d.Ativo)
                .ToListAsync();
        }

        public async Task AdicionarAsync(RedirecionamentoDestino destino)
        {
            _context.RedirecionamentoDestinos.Add(destino);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(RedirecionamentoDestino destino)
        {
            _context.RedirecionamentoDestinos.Update(destino);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var entity = await _context.RedirecionamentoDestinos.FindAsync(id);
            if (entity != null)
            {
                _context.RedirecionamentoDestinos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
