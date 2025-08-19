// Pagina.cs (Domain)
using System;
using System.Collections.Generic;
using Paginas.Domain.Enums;

namespace Paginas.Domain.Entities
{
    public class Pagina
    {
        public int Codigo { get; private set; }
        public string Titulo { get; private set; }
        public string Conteudo { get; private set; }
        public string Url { get; private set; }
        public TipoPagina Tipo { get; private set; }
        public int? CdPai { get; private set; }
        public DateTime Criacao { get; private set; }
        public DateTime? Atualizacao { get; private set; }
        public bool Publicacao { get; private set; }
        public bool Status { get; private set; }
        public int Ordem { get; private set; }
        public int Versao { get; private set; }
        public string Banner { get; private set; }
        public int? CdVersao { get; private set; }

        // Navegação
        public Pagina PaginaPai { get; private set; }
        public List<Pagina> PaginaFilhos { get; private set; } = new();
        public List<Botao> Botoes { get; private set; } = new();

        protected Pagina() { } // EF Core

        public Pagina(string titulo, string conteudo, string url, TipoPagina tipo, int? cdPai = null)
        {
            Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
            Conteudo = conteudo ?? throw new ArgumentNullException(nameof(conteudo));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Tipo = tipo;
            CdPai = cdPai;
            Criacao = DateTime.UtcNow;
            Publicacao = false;
            Status = true;
            Ordem = 0;
            Versao = 1;
        }

        public void Atualizar(string titulo, string conteudo, string url, TipoPagina tipo)
        {
            Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
            Conteudo = conteudo ?? throw new ArgumentNullException(nameof(conteudo));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Tipo = tipo;
            Atualizacao = DateTime.UtcNow;
            Versao++;
        }

        // adiciona no agregado Pagina
        public void DefinirBanner(string banner)
        {
            // aceita null/empty para limpar o banner também
            Banner = string.IsNullOrWhiteSpace(banner) ? null : banner.Trim();
            Atualizacao = DateTime.UtcNow;
            Versao++;
        }

        public void AtualizarOrdem(int novaOrdem)
        {
            Ordem = novaOrdem;
            Atualizacao = DateTime.UtcNow;
            Versao++;
        }

        public void Publicar() => Publicacao = true;
        public void Despublicar() => Publicacao = false;
        public void Ativar() => Status = true;
        public void Desativar() => Status = false;

        public void AdicionarBotao(Botao botao)
        {
            if (botao == null) throw new ArgumentNullException(nameof(botao));
            Botoes.Add(botao);
        }

        public void RemoverBotao(Botao botao)
        {
            if (botao == null) throw new ArgumentNullException(nameof(botao));
            Botoes.Remove(botao);
        }
    }
}
