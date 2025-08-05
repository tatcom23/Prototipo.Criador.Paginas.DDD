using Paginas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
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

        public async Task<List<Pagina>> ListarAsync()
        {
            return await _context.Paginas
                .Include(p => p.Botoes)
                .Include(p => p.PaginaFilhos) // <- Carrega os tópicos
                .ThenInclude(t => t.Botoes)   // <- E os botões dos tópicos
                .ToListAsync();
        }

        public async Task<Pagina> BuscarPorIdAsync(int id)
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

        public async Task ExcluirAsync(Pagina pagina)
        {
            // Exclui os botões da página
            _context.Botoes.RemoveRange(pagina.Botoes);

            // Exclui os botões dos tópicos também
            foreach (var topico in pagina.PaginaFilhos)
            {
                _context.Botoes.RemoveRange(topico.Botoes);
            }

            // Exclui os tópicos filhos
            _context.Paginas.RemoveRange(pagina.PaginaFilhos);

            // Exclui a própria página
            _context.Paginas.Remove(pagina);
        }

        public async Task<List<Pagina>> ListarFilhosAsync(int cdPai)
        {
            return await _context.Paginas
                .Where(p => p.CdPai == cdPai)
                .Include(p => p.Botoes)
                .ToListAsync();
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
