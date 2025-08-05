using Paginas.Domain.Entities;
using Paginas.Application.DTOs;  // ✅ Para PaginaDTO
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Application.Services.Interfaces
{
    public interface IPaginaService
    {
        Task<List<Pagina>> ListarAsync();
        Task<Pagina> BuscarPorIdAsync(int id);
        Task CriarAsync(PaginaDTO model);
        Task AtualizarAsync(int id, PaginaDTO model);
        Task ExcluirAsync(int id);
        Task CriarComPaiAsync(PaginaDTO model, int cdPai);
        Task AtualizarOrdemAsync(Pagina a, Pagina b);
    }
}
