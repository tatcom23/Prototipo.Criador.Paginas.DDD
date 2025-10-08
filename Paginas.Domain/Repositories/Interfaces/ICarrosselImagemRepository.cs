using Paginas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Domain.Repositories.Interfaces
{
    public interface ICarrosselImagemRepository
    {
        Task<List<CarrosselImagem>> ListarPorCarrosselAsync(int cdCarrossel);
        Task<CarrosselImagem> ObterPorIdAsync(int id);

        Task AdicionarAsync(CarrosselImagem imagem);
        Task AtualizarAsync(CarrosselImagem imagem);
        Task RemoverAsync(CarrosselImagem imagem);

        Task SalvarAlteracoesAsync();
    }
}
