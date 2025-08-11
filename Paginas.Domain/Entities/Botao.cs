// Domain/Entities/Botao.cs
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

        protected Botao() { } // EF Core

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

        // ✅ Método estático para criar Botao a partir de BotaoDTO
        public static Botao FromDTO(Application.DTOs.BotaoDTO dto)
        {
            if (dto == null) return null;

            var botao = new Botao(
                nome: dto.Nome,
                link: dto.Link,
                tipo: (TipoBotao)dto.Tipo,
                cdPaginaIntrodutoria: dto.CdPaginaIntrodutoria,
                linha: dto.Linha,
                coluna: dto.Coluna
            );

            // Usar reflection ou métodos internos para setar campos privados
            // Como setters são privados, só podemos setar via reflection ou se estiver no mesmo assembly
            typeof(Botao).GetProperty(nameof(Codigo))?.SetValue(botao, dto.Codigo);
            typeof(Botao).GetProperty(nameof(Criacao))?.SetValue(botao, dto.Criacao);
            typeof(Botao).GetProperty(nameof(Atualizacao))?.SetValue(botao, dto.Atualizacao);
            typeof(Botao).GetProperty(nameof(Status))?.SetValue(botao, dto.Status);
            typeof(Botao).GetProperty(nameof(Versao))?.SetValue(botao, dto.Versao);

            return botao;
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