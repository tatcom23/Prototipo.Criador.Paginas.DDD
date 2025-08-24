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
                ordem: dto.Ordem
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
                Ordem = entity.Ordem,
                Status = entity.Status,
                Criacao = entity.Criacao,
                Atualizacao = entity.Atualizacao
            };
        }
    }
}