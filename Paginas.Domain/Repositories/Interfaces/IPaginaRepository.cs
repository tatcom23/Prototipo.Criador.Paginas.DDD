using Paginas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Domain.Repositories.Interfaces
{
    public interface IPaginaRepository
    {
        Task<List<Pagina>> ListarTodasAsync();
        Task<Pagina> ObterPorIdAsync(int id);
        Task<List<Pagina>> ListarFilhosAsync(int cdPai);
        Task AdicionarAsync(Pagina pagina);
        Task AtualizarAsync(Pagina pagina);
        Task RemoverAsync(Pagina pagina);
        Task SalvarAlteracoesAsync();

        // Paginação no banco: retorna itens e o total (count)
        Task<(List<Pagina> Items, int TotalCount)> ObterPaginadoAsync(int page, int pageSize, bool apenasRaiz = true);
    }
}
