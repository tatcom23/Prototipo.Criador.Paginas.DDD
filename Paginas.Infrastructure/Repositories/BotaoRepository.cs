using Microsoft.EntityFrameworkCore;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using Paginas.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Infrastructure.Repositories
{
    public class BotaoRepository : IBotaoRepository
    {
        private readonly AppDbContext _context;

        public BotaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Botao> ObterPorIdAsync(int id)
        {
            return await _context.Botoes.FirstOrDefaultAsync(b => b.Codigo == id);
        }

        public async Task<List<Botao>> ListarAsync()
        {
            return await _context.Botoes.ToListAsync();
        }

        public async Task AdicionarAsync(Botao botao)
        {
            await _context.Botoes.AddAsync(botao);
        }

        public async Task AtualizarAsync(Botao botao)
        {
            _context.Botoes.Update(botao);
            await SalvarAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var botao = await ObterPorIdAsync(id);
            if (botao != null)
            {
                _context.Botoes.Remove(botao);
                await SalvarAsync();
            }
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
