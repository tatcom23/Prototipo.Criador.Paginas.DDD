using Redirect.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redirect.Application.Services.Interfaces
{
    public interface IRedirectURLService
    {
        Task<RedirectURLDTO?> ObterPorUrlAntigaAsync(string urlAntiga);
        Task<IEnumerable<RedirectURLDTO>> ObterTodosAsync();
        Task AdicionarAsync(RedirectURLDTO dto);
        Task AtualizarAsync(RedirectURLDTO dto);
        Task RemoverAsync(int id);
    }
}
