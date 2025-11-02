using System;
using System.Collections.Generic;

namespace Redirect.Domain.Entities
{
    public class RedirecionamentoOrigem
    {
        public int Codigo { get; set; }
        public string UrlOrigem { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime? DtRedirecionamento { get; set; }
        public DateTime? DtAtualizacao { get; set; }

        // 🔹 Relacionamento 1:N com os destinos
        public ICollection<RedirecionamentoDestino> Destinos { get; set; } = new List<RedirecionamentoDestino>();
    }
}
