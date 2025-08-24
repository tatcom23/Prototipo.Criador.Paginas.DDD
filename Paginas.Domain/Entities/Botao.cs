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
        public int Ordem { get; private set; } // 👈 Novo campo
        public DateTime Criacao { get; private set; }
        public DateTime? Atualizacao { get; private set; }
        public bool Status { get; private set; }

        // Navegação
        public Pagina Pagina { get; private set; }

        public Botao() { } // EF Core

        public Botao(string nome, string link, TipoBotao tipo, int cdPaginaIntrodutoria, int ordem = 1)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Link = link ?? throw new ArgumentNullException(nameof(link));
            Tipo = tipo;
            CdPaginaIntrodutoria = cdPaginaIntrodutoria;
            Ordem = ordem;
            Criacao = DateTime.UtcNow;
            Status = true;
        }

        public Botao(int codigo, string nome, string link, TipoBotao tipo, int cdPaginaIntrodutoria, int ordem = 1)
            : this(nome, link, tipo, cdPaginaIntrodutoria, ordem)
        {
            Codigo = codigo;
        }

        public void Atualizar(string nome, string link, TipoBotao tipo, int ordem)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Link = link ?? throw new ArgumentNullException(nameof(link));
            Tipo = tipo;
            Ordem = ordem;
            Atualizacao = DateTime.UtcNow;
        }

        public void Ativar() => Status = true;
        public void Desativar() => Status = false;
    }
}