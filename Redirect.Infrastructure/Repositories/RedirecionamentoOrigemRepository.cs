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
                .Where(r => r.Ativo)
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
                // 🔹 Não exclui fisicamente — apenas desativa
                entity.Ativo = false;
                _context.RedirecionamentoOrigens.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        // 🔹 Implementação da paginação
        public async Task<(IEnumerable<RedirecionamentoOrigem> Itens, int TotalItens)> ObterPaginadoAsync(int page, int pageSize)
        {
            if (pageSize != 5 && pageSize != 10 && pageSize != 15 && pageSize != 20)
                pageSize = 10; // valor padrão seguro

            var query = _context.RedirecionamentoOrigens
                .Include(r => r.Destinos)
                .Where(r => r.Ativo)
                .OrderByDescending(r => r.DtAtualizacao)
                .AsNoTracking();

            var totalItens = await query.CountAsync();

            var itens = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (itens, totalItens);
        }
    }
}
