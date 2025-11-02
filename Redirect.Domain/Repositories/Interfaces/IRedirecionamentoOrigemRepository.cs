using Redirect.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redirect.Domain.Repositories.Interfaces
{
    public interface IRedirecionamentoOrigemRepository
    {
        Task<RedirecionamentoOrigem?> ObterPorIdAsync(int id);
        Task<RedirecionamentoOrigem?> ObterPorUrlOrigemAsync(string urlOrigem);
        Task<IEnumerable<RedirecionamentoOrigem>> ObterTodosAsync();
        Task AdicionarAsync(RedirecionamentoOrigem redirecionamentoOrigem);
        Task AtualizarAsync(RedirecionamentoOrigem redirecionamentoOrigem);
        Task RemoverAsync(int id);

        // 🔹 Inclui os destinos
        Task<RedirecionamentoOrigem?> ObterComDestinosAsync(string urlOrigem);
    }
}
