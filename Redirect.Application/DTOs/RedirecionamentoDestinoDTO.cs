using System;

namespace Redirect.Application.DTOs
{
    public class RedirecionamentoDestinoDTO
    {
        public int Codigo { get; set; }
        public int RedirecionamentoOrigemId { get; set; }
        public string UrlDestino { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime? DtInicial { get; set; }
        public DateTime? DtFinal { get; set; }
    }
}