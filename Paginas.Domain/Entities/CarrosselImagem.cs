using System;

namespace Paginas.Domain.Entities
{
    public class CarrosselImagem
    {
        public int Codigo { get; private set; }
        public int CdCarrossel { get; private set; }
        public string? Titulo { get; private set; }
        public string? Descricao { get; private set; }
        public string UrlImagem { get; private set; }
        public int Ordem { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime Criacao { get; private set; }
        public DateTime? Alteracao { get; private set; }

        // Navegação
        public Carrossel Carrossel { get; private set; } = null!;

        protected CarrosselImagem() { } // EF Core

        public CarrosselImagem(int cdCarrossel, string urlImagem, int ordem, string? titulo = null, string? descricao = null)
        {
            if (string.IsNullOrWhiteSpace(urlImagem))
                throw new ArgumentNullException(nameof(urlImagem));

            CdCarrossel = cdCarrossel;
            UrlImagem = urlImagem.Trim();
            Ordem = ordem;

            Titulo = string.IsNullOrWhiteSpace(titulo) ? null : titulo.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            Ativo = true;
            Criacao = DateTime.UtcNow;
        }

        public void Atualizar(string urlImagem, int ordem, string? titulo, string? descricao)
        {
            if (string.IsNullOrWhiteSpace(urlImagem))
                throw new ArgumentNullException(nameof(urlImagem));

            UrlImagem = urlImagem.Trim();
            Ordem = ordem;

            Titulo = string.IsNullOrWhiteSpace(titulo) ? null : titulo.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            Alteracao = DateTime.UtcNow;
        }

        public void Ativar() => Ativo = true;
        public void Desativar() => Ativo = false;
    }
}
