using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using Paginas.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paginas.Infrastructure.Repositories
{
    public class CarrosselImagemRepository : ICarrosselImagemRepository
    {
        private readonly AppDbContext _context;

        public CarrosselImagemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarrosselImagem>> ListarPorCarrosselAsync(int cdCarrossel)
        {
            return await _context.CarrosselImagens
                .AsNoTracking()
                .Where(i => i.CdCarrossel == cdCarrossel)
                .ToListAsync();
        }

        public async Task<CarrosselImagem> ObterPorIdAsync(int id)
        {
            return await _context.CarrosselImagens
                .FirstOrDefaultAsync(i => i.Codigo == id);
        }

        public async Task AdicionarAsync(CarrosselImagem imagem)
        {
            await _context.CarrosselImagens.AddAsync(imagem);
        }

        public async Task AtualizarAsync(CarrosselImagem imagem)
        {
            _context.CarrosselImagens.Update(imagem);
        }

        public async Task RemoverAsync(CarrosselImagem imagem)
        {
            _context.CarrosselImagens.Remove(imagem);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
