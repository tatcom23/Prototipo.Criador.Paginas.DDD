// Application → Mappers/PaginaMapper.cs
using Paginas.Application.DTOs;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using System.Linq;

namespace Paginas.Application.Mappers
{
    public static class PaginaMapper
    {
        public static Pagina ToEntity(this PaginaDTO dto)
        {
            if (dto == null) return null;

            var pagina = new Pagina(
                titulo: dto.Titulo,
                conteudo: dto.Conteudo,
                url: dto.Url,
                tipo: (TipoPagina)dto.Tipo,
                cdPai: dto.CdPai
            );

            // Forçar valores privados via reflection (se necessário)
            typeof(Pagina).GetProperty(nameof(Pagina.Codigo))?.SetValue(pagina, dto.Codigo);
            typeof(Pagina).GetProperty(nameof(Pagina.Status))?.SetValue(pagina, dto.Status);
            typeof(Pagina).GetProperty(nameof(Pagina.Publicacao))?.SetValue(pagina, dto.Publicacao);

            // Mapear botões
            if (dto.Botoes != null)
            {
                foreach (var botaoDto in dto.Botoes)
                {
                    pagina.AdicionarBotao(botaoDto.ToEntity());
                }
            }

            // Mapear filhos
            if (dto.PaginaFilhos != null)
            {
                foreach (var filhoDto in dto.PaginaFilhos)
                {
                    pagina.PaginaFilhos.Add(filhoDto.ToEntity());
                }
            }

            return pagina;
        }

        public static PaginaDTO ToDTO(this Pagina entity)
        {
            if (entity == null) return null;

            return new PaginaDTO
            {
                Codigo = entity.Codigo,
                Titulo = entity.Titulo,
                Conteudo = entity.Conteudo,
                Url = entity.Url,
                Tipo = (int)entity.Tipo,
                CdPai = entity.CdPai,
                Publicacao = entity.Publicacao,
                Status = entity.Status,
                Ordem = entity.Ordem,
                Versao = entity.Versao,
                Banner = entity.Banner,
                Botoes = entity.Botoes?.Select(b => b.ToDTO()).ToList(),
                PaginaFilhos = entity.PaginaFilhos?.Select(f => f.ToDTO()).ToList()
            };
        }
    }
}
