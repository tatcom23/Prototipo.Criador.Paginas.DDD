using Redirect.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redirect.Application.Services.Interfaces
{
    public interface IRedirecionamentoOrigemService
    {
        Task<RedirecionamentoOrigemDTO?> ObterPorUrlOrigemAsync(string urlOrigem);
        Task<IEnumerable<RedirecionamentoOrigemDTO>> ObterTodosAsync();
        Task AdicionarAsync(RedirecionamentoOrigemDTO dto);
        Task AtualizarAsync(RedirecionamentoOrigemDTO dto);
        Task<RedirecionamentoOrigemDTO?> ObterPorIdAsync(int id);
        Task RemoverAsync(int id);

        // 🔹 Novo método para selecionar destino válido (usado pelo middleware)
        RedirecionamentoDestinoDTO? SelecionarDestinoValido(RedirecionamentoOrigemDTO origem);
    }
}
