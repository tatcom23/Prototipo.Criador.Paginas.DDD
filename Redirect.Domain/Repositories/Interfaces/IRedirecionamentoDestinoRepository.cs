using Redirect.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redirect.Domain.Repositories.Interfaces
{
    public interface IRedirecionamentoDestinoRepository
    {
        Task<RedirecionamentoDestino?> ObterPorIdAsync(int id);
        Task<IEnumerable<RedirecionamentoDestino>> ObterPorOrigemAsync(int origemId);
        Task AdicionarAsync(RedirecionamentoDestino destino);
        Task AtualizarAsync(RedirecionamentoDestino destino);
        Task RemoverAsync(int id);
    }
}
