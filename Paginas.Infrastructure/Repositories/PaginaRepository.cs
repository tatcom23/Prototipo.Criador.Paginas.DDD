using Paginas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Paginas.Domain.Repositories.Interfaces;
using Paginas.Infrastructure.Data.Context;

namespace Paginas.Infrastructure.Repositories
{
    public class PaginaRepository : IPaginaRepository
    {
        private readonly AppDbContext _context;

        public PaginaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pagina>> ListarTodasAsync()
        {
            return await _context.Paginas
                .Include(p => p.Botoes)
                .Include(p => p.PaginaFilhos)
                    .ThenInclude(t => t.Botoes)
                .ToListAsync();
        }

        public async Task<Pagina> ObterPorIdAsync(int id)
        {
            return await _context.Paginas
                .Include(p => p.Botoes)
                .Include(p => p.PaginaFilhos)
                    .ThenInclude(t => t.Botoes)
                .FirstOrDefaultAsync(p => p.Codigo == id);
        }

        public async Task AdicionarAsync(Pagina pagina)
        {
            await _context.Paginas.AddAsync(pagina);
        }

        public async Task AtualizarAsync(Pagina pagina)
        {
            _context.Paginas.Update(pagina);
        }

        public async Task RemoverAsync(Pagina pagina)
        {
            _context.Botoes.RemoveRange(pagina.Botoes);

            foreach (var topico in pagina.PaginaFilhos)
                _context.Botoes.RemoveRange(topico.Botoes);

            _context.Paginas.RemoveRange(pagina.PaginaFilhos);
            _context.Paginas.Remove(pagina);
        }

        public async Task<List<Pagina>> ListarFilhosAsync(int cdPai)
        {
            return await _context.Paginas
                .Where(p => p.CdPai == cdPai)
                .Include(p => p.Botoes)
                .ToListAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
