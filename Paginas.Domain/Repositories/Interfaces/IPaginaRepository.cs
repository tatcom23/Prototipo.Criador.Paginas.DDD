using Paginas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Domain.Repositories.Interfaces
{
    public interface IPaginaRepository
    {
        Task<List<Pagina>> ListarAsync();
        Task<Pagina> BuscarPorIdAsync(int id);
        Task AdicionarAsync(Pagina pagina);
        Task AtualizarAsync(Pagina pagina);
        Task ExcluirAsync(Pagina pagina);
        Task<List<Pagina>> ListarFilhosAsync(int cdPai);
        Task SalvarAsync();
    }
}

