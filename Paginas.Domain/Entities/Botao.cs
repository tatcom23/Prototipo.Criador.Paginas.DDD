using System;

namespace Paginas.Domain.Entities
{
    public class Botao
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Link { get; set; }
        public int Tipo { get; set; }
        public int CdPaginaIntrodutoria { get; set; }
        public int Linha { get; set; }
        public int Coluna { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Atualizacao { get; set; }
        public bool Status { get; set; }
        public int Versao { get; set; }

        // Navegação para Pagina
        public Pagina Pagina { get; set; }
    }
}
