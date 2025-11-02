using System;
using System.Collections.Generic;

namespace Redirect.Application.DTOs
{
    public class RedirecionamentoOrigemDTO
    {
        public int Codigo { get; set; }
        public string UrlOrigem { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime? DtRedirecionamento { get; set; }
        public DateTime? DtAtualizacao { get; set; }
        public List<RedirecionamentoDestinoDTO> Destinos { get; set; } = new();
    }
}
