using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redirect.Domain.Entities
{
    public class RedirectURL
    {
        public int Codigo { get; set; }
        public string UrlAntiga { get; set; } = string.Empty;
        public string UrlNova { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime? DtRedirectUrl { get; set; }
        public DateTime? DtAtualizacao { get; set; }
    }
}
