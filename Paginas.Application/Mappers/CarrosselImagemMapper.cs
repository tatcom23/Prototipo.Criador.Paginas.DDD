using Paginas.Application.DTOs;
using Paginas.Domain.Entities;

namespace Paginas.Application.Mappers
{
    public static class CarrosselImagemMapper
    {
        public static CarrosselImagem ToEntity(this CarrosselImagemDTO dto)
        {
            if (dto == null) return null;

            var entity = new CarrosselImagem(
                cdCarrossel: dto.CdCarrossel,
                urlImagem: dto.UrlImagem,
                ordem: dto.Ordem,
                titulo: dto.Titulo,
                descricao: dto.Descricao
            );

            // Forçar valores privados
            typeof(CarrosselImagem).GetProperty(nameof(CarrosselImagem.Codigo))?.SetValue(entity, dto.Codigo);
            typeof(CarrosselImagem).GetProperty(nameof(CarrosselImagem.Ativo))?.SetValue(entity, dto.Ativo);

            return entity;
        }

        public static CarrosselImagemDTO ToDTO(this CarrosselImagem entity)
        {
            if (entity == null) return null;

            return new CarrosselImagemDTO
            {
                Codigo = entity.Codigo,
                UrlImagem = entity.UrlImagem,
                Titulo = entity.Titulo,
                Descricao = entity.Descricao,
                Ordem = entity.Ordem,
                Ativo = entity.Ativo,
                Criacao = entity.Criacao,
                Alteracao = entity.Alteracao,
                CdCarrossel = entity.CdCarrossel
            };
        }
    }
}
