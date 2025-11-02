using Redirect.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redirect.Application.Services.Interfaces
{
    public interface IRedirecionamentoDestinoService
    {
        Task<IEnumerable<RedirecionamentoDestinoDTO>> ObterPorOrigemAsync(int origemId);
        Task<RedirecionamentoDestinoDTO?> ObterPorIdAsync(int id);
        Task AdicionarAsync(RedirecionamentoDestinoDTO dto);
        Task AtualizarAsync(RedirecionamentoDestinoDTO dto);
        Task RemoverAsync(int id);
    }
}
