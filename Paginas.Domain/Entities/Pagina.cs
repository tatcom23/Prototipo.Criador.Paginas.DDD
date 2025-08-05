using System;
using System.Collections.Generic;

namespace Paginas.Domain.Entities
{
    public class Pagina
    {
        public int Codigo { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string Url { get; set; }
        public int Tipo { get; set; }
        public int? CdPai { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Atualizacao { get; set; }
        public bool Publicacao { get; set; }
        public bool Status { get; set; }
        public int Ordem { get; set; }
        public int Versao { get; set; }
        public string Banner { get; set; }
        public int? CdVersao { get; set; }

        // Navegação
        public Pagina PaginaPai { get; set; }
        public List<Pagina> PaginaFilhos { get; set; } = new List<Pagina>();
        public List<Botao> Botoes { get; set; } = new List<Botao>();
    }
}
