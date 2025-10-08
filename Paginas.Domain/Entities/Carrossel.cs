using System;
using System.Collections.Generic;

namespace Paginas.Domain.Entities
{
    public class Carrossel
    {
        public int Codigo { get; private set; }
        public int CdPagina { get; private set; }
        public string? Titulo { get; private set; }
        public string? Descricao { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime Criacao { get; private set; }
        public DateTime? Alteracao { get; private set; }

        // Navegação
        public Pagina Pagina { get; private set; } = null!;
        public List<CarrosselImagem> Imagens { get; private set; } = new();

        protected Carrossel() { } // EF Core

        public Carrossel(int cdPagina, string? titulo, string? descricao)
        {
            CdPagina = cdPagina;
            Titulo = string.IsNullOrWhiteSpace(titulo) ? null : titulo.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            Ativo = true;
            Criacao = DateTime.UtcNow;
        }

        public void Atualizar(string? titulo, string? descricao)
        {
            Titulo = string.IsNullOrWhiteSpace(titulo) ? null : titulo.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            Alteracao = DateTime.UtcNow;
        }

        public void Ativar() => Ativo = true;
        public void Desativar() => Ativo = false;

        public void AdicionarImagem(CarrosselImagem imagem)
        {
            if (imagem == null) throw new ArgumentNullException(nameof(imagem));
            Imagens.Add(imagem);
        }

        public void RemoverImagem(CarrosselImagem imagem)
        {
            if (imagem == null) throw new ArgumentNullException(nameof(imagem));
            Imagens.Remove(imagem);
        }
        public void VincularPagina(int paginaId)
        {
            if (paginaId <= 0) throw new ArgumentException("Página inválida", nameof(paginaId));
            CdPagina = paginaId;
        }

    }
}
