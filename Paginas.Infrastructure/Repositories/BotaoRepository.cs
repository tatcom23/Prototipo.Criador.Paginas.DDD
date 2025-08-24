using Microsoft.EntityFrameworkCore;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using Paginas.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Linq;
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
            return await _context.Botoes
                .FirstOrDefaultAsync(b => b.Codigo == id);
        }

        public async Task<List<Botao>> ListarTodosAsync()
        {
            return await _context.Botoes.ToListAsync();
        }

        public async Task<List<Botao>> ListarPorPaginaAsync(int cdPagina)
        {
            return await _context.Botoes
                .Where(b => b.CdPaginaIntrodutoria == cdPagina)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Botao botao)
        {
            await _context.Botoes.AddAsync(botao);
        }

        public async Task AtualizarAsync(Botao botao)
        {
            _context.Botoes.Update(botao);
        }

        public async Task RemoverAsync(int id)
        {
            var botao = await ObterPorIdAsync(id);
            if (botao != null)
                _context.Botoes.Remove(botao);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // ✅ Método adicionado corretamente dentro da classe
        public async Task<int> ObterMaxOrdemPorPaginaAsync(int cdPaginaIntrodutoria)
        {
            var max = await _context.Botoes
                .Where(b => b.CdPaginaIntrodutoria == cdPaginaIntrodutoria)
                .MaxAsync(b => (int?)b.Ordem); // Usa (int?) para lidar com coleção vazia

            return max ?? 0; // Se não houver botões, retorna 0
        }
    }
}