using Paginas.Application.DTOs;
using System.Threading.Tasks;

namespace Paginas.Application.Services.Interfaces
{
    public interface IBotaoService
    {
        Task<BotaoDTO> BuscarPorIdAsync(int id);
        Task AtualizarAsync(BotaoDTO botaoDto);
        Task ExcluirAsync(int id);
        Task AtualizarOrdemAsync(int idA, int idB);
        Task AtualizarOrdemIndividualAsync(int botaoId, int novaOrdem);
        Task CriarAsync(BotaoDTO dto, int cdPaginaIntrodutoria);
    }
}
