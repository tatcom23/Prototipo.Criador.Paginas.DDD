using System;
using Paginas.Domain.Enums;

namespace Paginas.Application.DTOs
{
    public class BotaoDTO
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Link { get; set; }
        public int Tipo { get; set; }
        public int CdPaginaIntrodutoria { get; set; }
        public int Ordem { get; set; } // 👈 Substitui Linha/Coluna
        public bool Status { get; set; }
        public int Versao { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Atualizacao { get; set; }
    }
}