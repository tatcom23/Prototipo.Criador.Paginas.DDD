using System;
using Paginas.Domain.Enums;

namespace Paginas.Domain.Entities
{
    public class Botao
    {
        public int Codigo { get; private set; }
        public string Nome { get; private set; }
        public string Link { get; private set; }
        public TipoBotao Tipo { get; private set; }
        public int CdPaginaIntrodutoria { get; private set; }
        public int Linha { get; private set; }
        public int Coluna { get; private set; }
        public DateTime Criacao { get; private set; }
        public DateTime? Atualizacao { get; private set; }
        public bool Status { get; private set; }
        public int Versao { get; private set; }

        // Navegação
        public Pagina Pagina { get; private set; }

        public Botao() { } // EF Core e testes

        public Botao(string nome, string link, TipoBotao tipo, int cdPaginaIntrodutoria, int linha, int coluna)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Link = link ?? throw new ArgumentNullException(nameof(link));
            Tipo = tipo;
            CdPaginaIntrodutoria = cdPaginaIntrodutoria;
            Linha = linha;
            Coluna = coluna;
            Criacao = DateTime.UtcNow;
            Status = true;
            Versao = 1;
        }

        public Botao(int codigo, string nome, string link, TipoBotao tipo, int cdPaginaIntrodutoria, int linha, int coluna)
            : this(nome, link, tipo, cdPaginaIntrodutoria, linha, coluna)
        {
            Codigo = codigo;
        }

        public void Atualizar(string nome, string link, TipoBotao tipo, int linha, int coluna)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Link = link ?? throw new ArgumentNullException(nameof(link));
            Tipo = tipo;
            Linha = linha;
            Coluna = coluna;
            Atualizacao = DateTime.UtcNow;
            Versao++;
        }

        public void Ativar() => Status = true;
        public void Desativar() => Status = false;
    }
}
