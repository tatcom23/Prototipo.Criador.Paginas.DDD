using System;

namespace Redirect.Domain.Entities
{
    public class RedirecionamentoDestino
    {
        public int Codigo { get; set; }

        // 🔹 FK para a URL de origem
        public int RedirecionamentoOrigemId { get; set; }

        public string UrlDestino { get; set; } = string.Empty;
        public DateTime? DtInicial { get; set; }
        public DateTime? DtFinal { get; set; }
        public bool Ativo { get; set; } = true;

        // 🔹 Navegação inversa
        public RedirecionamentoOrigem? RedirecionamentoOrigem { get; set; }
    }
}
