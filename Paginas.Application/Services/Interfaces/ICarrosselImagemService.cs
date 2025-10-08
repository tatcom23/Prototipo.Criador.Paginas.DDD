using Paginas.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Application.Services.Interfaces
{
    public interface ICarrosselImagemService
    {
        Task<List<CarrosselImagemDTO>> ListarPorCarrosselAsync(int cdCarrossel);
        Task<CarrosselImagemDTO> ObterPorIdAsync(int id);

        // Método principal
        Task CriarAsync(CarrosselImagemDTO model);

        // Nova sobrecarga para criar com referência ao Carrossel
        Task<CarrosselImagemDTO> CriarAsync(CarrosselImagemDTO model, int cdCarrossel);

        Task AtualizarAsync(int id, CarrosselImagemDTO model);
        Task ExcluirAsync(int id);

        Task AtivarAsync(int id);
        Task DesativarAsync(int id);

        Task AtualizarOrdemAsync(int idA, int idB);
    }
}
