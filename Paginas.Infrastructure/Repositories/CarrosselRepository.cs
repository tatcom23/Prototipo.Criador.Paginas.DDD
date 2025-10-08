using Microsoft.EntityFrameworkCore;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using Paginas.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paginas.Infrastructure.Repositories
{
    public class CarrosselRepository : ICarrosselRepository
    {
        private readonly AppDbContext _context;

        public CarrosselRepository(AppDbContext context)
        {
            _context = context;
        }

        // CARROSSEL
        public async Task<List<Carrossel>> ListarTodosAsync()
        {
            return await _context.Carrosseis
                .Include(c => c.Imagens)
                .ToListAsync();
        }

        public async Task<Carrossel> ObterPorIdAsync(int id)
        {
            return await _context.Carrosseis
                .Include(c => c.Imagens)
                .FirstOrDefaultAsync(c => c.Codigo == id);
        }

        public async Task<List<Carrossel>> ListarPorPaginaAsync(int cdPagina)
        {
            return await _context.Carrosseis
                .Where(c => c.CdPagina == cdPagina)
                .Include(c => c.Imagens)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Carrossel carrossel)
        {
            await _context.Carrosseis.AddAsync(carrossel);
        }

        public async Task AtualizarAsync(Carrossel carrossel)
        {
            _context.Carrosseis.Update(carrossel);
        }

        public async Task RemoverAsync(Carrossel carrossel)
        {
            _context.CarrosselImagens.RemoveRange(carrossel.Imagens);
            _context.Carrosseis.Remove(carrossel);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // IMAGENS
        public async Task<CarrosselImagem> ObterImagemPorIdAsync(int id)
        {
            return await _context.CarrosselImagens
                .Include(i => i.Carrossel)
                .FirstOrDefaultAsync(i => i.Codigo == id);
        }

        public async Task AdicionarImagemAsync(CarrosselImagem imagem)
        {
            await _context.CarrosselImagens.AddAsync(imagem);
        }

        public async Task AtualizarImagemAsync(CarrosselImagem imagem)
        {
            _context.CarrosselImagens.Update(imagem);
        }

        public async Task RemoverImagemAsync(CarrosselImagem imagem)
        {
            _context.CarrosselImagens.Remove(imagem);
        }
    }
}
