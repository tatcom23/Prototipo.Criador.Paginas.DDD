using Paginas.Application.DTOs;
using System.Threading.Tasks;

namespace Paginas.Application.Services.Interfaces
{
    public interface IBotaoService
    {
        Task<BotaoDTO> BuscarPorIdAsync(int id);
        Task AtualizarAsync(BotaoDTO botaoDto);
        Task ExcluirAsync(int id);
    }
}
