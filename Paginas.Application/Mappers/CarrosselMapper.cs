using Paginas.Application.DTOs;
using Paginas.Domain.Entities;
using System.Linq;

namespace Paginas.Application.Mappers
{
    public static class CarrosselMapper
    {
        public static Carrossel ToEntity(this CarrosselDTO dto)
        {
            if (dto == null) return null;

            var entity = new Carrossel(
                cdPagina: dto.CdPagina,
                titulo: dto.Titulo,
                descricao: dto.Descricao
            );

            // Forçar valores privados se necessário
            typeof(Carrossel).GetProperty(nameof(Carrossel.Codigo))?.SetValue(entity, dto.Codigo);
            typeof(Carrossel).GetProperty(nameof(Carrossel.Ativo))?.SetValue(entity, dto.Ativo);

            // Mapear imagens
            if (dto.Imagens != null)
            {
                foreach (var imgDto in dto.Imagens)
                {
                    entity.AdicionarImagem(imgDto.ToEntity());
                }
            }

            return entity;
        }

        public static CarrosselDTO ToDTO(this Carrossel entity)
        {
            if (entity == null) return null;

            return new CarrosselDTO
            {
                Codigo = entity.Codigo,
                Titulo = entity.Titulo,
                Descricao = entity.Descricao,
                Ativo = entity.Ativo,
                Criacao = entity.Criacao,
                Alteracao = entity.Alteracao,
                CdPagina = entity.CdPagina,
                Imagens = entity.Imagens?.Select(i => i.ToDTO()).ToList()
            };
        }
    }
}
