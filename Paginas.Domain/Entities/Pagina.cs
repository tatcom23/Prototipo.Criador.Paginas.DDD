// Domain/Entities/Pagina.cs
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

        // ✅ Método estático para criar Pagina a partir de PaginaDTO
        public static Pagina FromDTO(Application.DTOs.PaginaDTO dto)
        {
            if (dto == null) return null;

            var pagina = new Pagina(
                titulo: dto.Titulo,
                conteudo: dto.Conteudo,
                url: dto.Url,
                tipo: (TipoPagina)dto.Tipo,
                cdPai: dto.CdPai
            );

            // Usar reflection para setar propriedades privadas
            var type = typeof(Pagina);
            type.GetProperty(nameof(Codigo))?.SetValue(pagina, dto.Codigo);
            type.GetProperty(nameof(Criacao))?.SetValue(pagina, dto.Criacao);
            type.GetProperty(nameof(Atualizacao))?.SetValue(pagina, dto.Atualizacao);
            type.GetProperty(nameof(Publicacao))?.SetValue(pagina, dto.Publicacao);
            type.GetProperty(nameof(Status))?.SetValue(pagina, dto.Status);
            type.GetProperty(nameof(Ordem))?.SetValue(pagina, dto.Ordem);
            type.GetProperty(nameof(Versao))?.SetValue(pagina, dto.Versao);
            type.GetProperty(nameof(Banner))?.SetValue(pagina, dto.Banner);
            type.GetProperty(nameof(CdVersao))?.SetValue(pagina, dto.CdVersao);

            // Mapear botões
            if (dto.Botoes != null)
            {
                foreach (var botaoDto in dto.Botoes)
                {
                    var botao = Botao.FromDTO(botaoDto);
                    if (botao != null)
                    {
                        pagina.Botoes.Add(botao);
                    }
                }
            }

            // Mapear filhos (recursivo)
            if (dto.PaginaFilhos != null)
            {
                foreach (var filhoDto in dto.PaginaFilhos)
                {
                    var filho = FromDTO(filhoDto);
                    if (filho != null)
                    {
                        pagina.PaginaFilhos.Add(filho);
                    }
                }
            }

            return pagina;
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