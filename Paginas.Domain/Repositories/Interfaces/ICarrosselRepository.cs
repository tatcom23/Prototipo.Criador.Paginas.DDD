using Paginas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Domain.Repositories.Interfaces
{
    public interface ICarrosselRepository
    {
        Task<List<Carrossel>> ListarTodosAsync();
        Task<Carrossel> ObterPorIdAsync(int id);
        Task<List<Carrossel>> ListarPorPaginaAsync(int cdPagina);

        Task AdicionarAsync(Carrossel carrossel);
        Task AtualizarAsync(Carrossel carrossel);
        Task RemoverAsync(Carrossel carrossel);
        Task SalvarAlteracoesAsync();
        
        // Imagens
        Task<CarrosselImagem> ObterImagemPorIdAsync(int id);
        Task AdicionarImagemAsync(CarrosselImagem imagem);
        Task AtualizarImagemAsync(CarrosselImagem imagem);
        Task RemoverImagemAsync(CarrosselImagem imagem);
    }
}
