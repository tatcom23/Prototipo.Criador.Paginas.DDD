using Paginas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Domain.Repositories.Interfaces
{
    public interface IBotaoRepository
    {
        Task<Botao> ObterPorIdAsync(int id);
        Task<List<Botao>> ListarAsync();
        Task AdicionarAsync(Botao botao);
        Task AtualizarAsync(Botao botao);
        Task ExcluirAsync(int id);
        Task SalvarAsync();
    }
}
