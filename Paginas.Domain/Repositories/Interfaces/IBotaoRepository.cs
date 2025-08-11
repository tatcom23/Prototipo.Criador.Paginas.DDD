using Paginas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Domain.Repositories.Interfaces
{
    public interface IBotaoRepository
    {
        Task<Botao> ObterPorIdAsync(int id);
        Task<List<Botao>> ListarTodosAsync();
        Task<List<Botao>> ListarPorPaginaAsync(int cdPagina); // consulta mais semântica
        Task AdicionarAsync(Botao botao);
        Task AtualizarAsync(Botao botao);
        Task RemoverAsync(int id);
        Task SalvarAlteracoesAsync();
    }
}
