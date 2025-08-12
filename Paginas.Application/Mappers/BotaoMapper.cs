using Paginas.Application.DTOs;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;

namespace Paginas.Application.Mappers
{
    public static class BotaoMapper
    {
        public static Botao ToEntity(this BotaoDTO dto)
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

            typeof(Botao).GetProperty(nameof(Botao.Codigo))?.SetValue(botao, dto.Codigo);
            typeof(Botao).GetProperty(nameof(Botao.Status))?.SetValue(botao, dto.Status);

            return botao;
        }

        public static BotaoDTO ToDTO(this Botao entity)
        {
            if (entity == null) return null;

            return new BotaoDTO
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Link = entity.Link,
                Tipo = (int)entity.Tipo,
                CdPaginaIntrodutoria = entity.CdPaginaIntrodutoria,
                Linha = entity.Linha,
                Coluna = entity.Coluna,
                Status = entity.Status,
                Versao = entity.Versao
            };
        }
    }
}
