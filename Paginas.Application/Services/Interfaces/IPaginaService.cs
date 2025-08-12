using Paginas.Domain.Entities;
using Paginas.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Application.Services.Interfaces
{
    public interface IPaginaService
    {
        Task<List<Pagina>> ListarAsync();
        Task<Pagina> BuscarPorIdAsync(int id);

        // Atualizado para aceitar o webRootPath, como no service
        Task CriarAsync(PaginaDTO model, string webRootPath);
        Task AtualizarAsync(int id, PaginaDTO model, string webRootPath);

        Task ExcluirAsync(int id);
        Task CriarComPaiAsync(PaginaDTO model, int cdPai);
        Task AtualizarOrdemAsync(Pagina a, Pagina b);
    }
}
