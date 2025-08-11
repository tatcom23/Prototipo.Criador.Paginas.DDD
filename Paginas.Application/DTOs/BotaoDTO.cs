// Application/DTOs/BotaoDTO.cs
using System;

namespace Paginas.Application.DTOs
{
    public class BotaoDTO
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Link { get; set; }
        public int Tipo { get; set; } // TipoBotao como int
        public int CdPaginaIntrodutoria { get; set; }
        public int Linha { get; set; }
        public int Coluna { get; set; }
        public bool Status { get; set; }
        public int Versao { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Atualizacao { get; set; }
    }
}