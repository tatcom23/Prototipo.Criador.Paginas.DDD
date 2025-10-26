using Redirect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redirect.Domain.Repositories.Interfaces
{
    public interface IRedirectURLRepository
    {
        Task<RedirectURL?> ObterPorIdAsync(int id);  // ✅ novo método
        Task<RedirectURL?> ObterPorUrlAntigaAsync(string urlAntiga);
        Task<IEnumerable<RedirectURL>> ObterTodosAsync();
        Task AdicionarAsync(RedirectURL redirectURL);
        Task AtualizarAsync(RedirectURL redirectURL);
        Task RemoverAsync(int id);
    }
}
