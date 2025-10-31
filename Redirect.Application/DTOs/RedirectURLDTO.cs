using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redirect.Application.DTOs
{
    public class RedirectURLDTO
    {
        public int Codigo { get; set; }
        public string UrlAntiga { get; set; } = string.Empty;
        public string UrlNova { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime? DtRedirectUrl { get; set; }
        public DateTime? DtAtualizacao { get; set; }
        public DateTime? DtInicial { get; set; }
        public DateTime? DtFinal { get; set; }
    }
}
