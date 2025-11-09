using Paginas.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Application.Services.Interfaces
{
    public interface ICarrosselService
    {
        Task<List<CarrosselDTO>> ListarPorPaginaAsync(int cdPagina);
        Task<CarrosselDTO> BuscarPorIdAsync(int id);

        // Cria carrossel vinculando a uma página específica
        Task<CarrosselDTO> CriarAsync(CarrosselDTO model, int paginaId);

        Task AtualizarAsync(int id, CarrosselDTO model);
        Task ExcluirAsync(int id);

        // Imagens via ICarrosselImagemService
        Task AdicionarImagemAsync(int cdCarrossel, CarrosselImagemDTO imagem);
        Task AtualizarImagemAsync(int id, CarrosselImagemDTO imagem);
        Task ExcluirImagemAsync(int id);
        Task AtualizarOrdemImagensAsync(int cdCarrossel, List<int> ordemIds);
        Task SalvarCarrosselCompletoAsync(CarrosselDTO model);
    }
}
