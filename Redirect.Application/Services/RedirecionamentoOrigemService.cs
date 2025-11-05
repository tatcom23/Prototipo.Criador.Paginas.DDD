using Redirect.Application.DTOs;
using Redirect.Application.Services.Interfaces;
using Redirect.Domain.Entities;
using Redirect.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redirect.Application.Services
{
    public class RedirecionamentoOrigemService : IRedirecionamentoOrigemService
    {
        private readonly IRedirecionamentoOrigemRepository _repository;
        private readonly IRedirecionamentoDestinoRepository _destinoRepository;

        public RedirecionamentoOrigemService(
            IRedirecionamentoOrigemRepository repository,
            IRedirecionamentoDestinoRepository destinoRepository)
        {
            _repository = repository;
            _destinoRepository = destinoRepository;
        }

        #region 🔹 Consultas

        public async Task<RedirecionamentoOrigemDTO?> ObterPorUrlOrigemAsync(string urlOrigem)
        {
            var entity = await _repository.ObterComDestinosAsync(urlOrigem);
            return entity == null ? null : MapearOrigemParaDTO(entity);
        }

        public async Task<RedirecionamentoOrigemDTO?> ObterPorIdAsync(int id)
        {
            var entity = await _repository.ObterPorIdAsync(id);
            return entity == null ? null : MapearOrigemParaDTO(entity);
        }

        public async Task<IEnumerable<RedirecionamentoOrigemDTO>> ObterTodosAsync()
        {
            var entities = await _repository.ObterTodosAsync();
            return entities
                .Where(e => e.Ativo)
                .Select(MapearOrigemParaDTO);
        }

        #endregion

        #region 🔹 CRUD Principal

        public async Task AdicionarAsync(RedirecionamentoOrigemDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            ValidarDestinos(dto.Destinos);

            var entity = new RedirecionamentoOrigem
            {
                UrlOrigem = dto.UrlOrigem,
                Ativo = true,
                DtRedirecionamento = dto.DtRedirecionamento ?? DateTime.Now,
                DtAtualizacao = DateTime.Now,
                Destinos = dto.Destinos?.Select(MapearDestinoParaEntity).ToList() ?? new List<RedirecionamentoDestino>()
            };

            await _repository.AdicionarAsync(entity);
        }

        public async Task AtualizarAsync(RedirecionamentoOrigemDTO dto)
        {
            var entityExistente = await _repository.ObterPorIdAsync(dto.Codigo);
            if (entityExistente == null)
                throw new Exception("Redirecionamento não encontrado.");

            entityExistente.UrlOrigem = dto.UrlOrigem;
            entityExistente.Ativo = dto.Ativo;
            entityExistente.DtAtualizacao = DateTime.Now;

            var destinosDto = dto.Destinos ?? new List<RedirecionamentoDestinoDTO>();
            ValidarDestinos(destinosDto);

            // 🔹 Atualiza ou adiciona destinos
            foreach (var destinoDto in destinosDto)
            {
                var destinoExistente = entityExistente.Destinos.FirstOrDefault(d => d.Codigo == destinoDto.Codigo);

                if (destinoExistente != null)
                {
                    destinoExistente.UrlDestino = destinoDto.UrlDestino;
                    destinoExistente.DtInicial = destinoDto.DtInicial;
                    destinoExistente.DtFinal = destinoDto.DtFinal;
                    destinoExistente.Ativo = destinoDto.Ativo;
                }
                else
                {
                    entityExistente.Destinos.Add(MapearDestinoParaEntity(destinoDto));
                }
            }

            // 🔹 Remove destinos que foram excluídos no formulário
            var destinosParaRemover = entityExistente.Destinos
                .Where(d => !destinosDto.Any(dto => dto.Codigo == d.Codigo))
                .ToList();

            foreach (var destino in destinosParaRemover)
                await _destinoRepository.RemoverAsync(destino.Codigo);

            await _repository.AtualizarAsync(entityExistente);
        }

        public async Task RemoverAsync(int id)
        {
            var entity = await _repository.ObterPorIdAsync(id);
            if (entity == null)
                throw new Exception("Redirecionamento não encontrado.");

            // 🔹 Marca a origem como inativa
            entity.Ativo = false;
            entity.DtAtualizacao = DateTime.Now;

            // 🔹 Também desativa todos os destinos associados
            if (entity.Destinos?.Any() == true)
            {
                foreach (var destino in entity.Destinos)
                    destino.Ativo = false;
            }

            await _repository.AtualizarAsync(entity);
        }

        #endregion

        #region 🔹 Métodos Auxiliares

        private void ValidarDestinos(IEnumerable<RedirecionamentoDestinoDTO>? destinos)
        {
            if (destinos == null || !destinos.Any())
                return;

            var destinosOrdenados = destinos
                .OrderBy(d => d.DtInicial ?? DateTime.MinValue)
                .ToList();

            for (int i = 0; i < destinosOrdenados.Count; i++)
            {
                var atual = destinosOrdenados[i];

                // 1️⃣ DtFinal < DtInicial
                if (atual.DtFinal.HasValue && atual.DtInicial.HasValue &&
                    atual.DtFinal.Value < atual.DtInicial.Value)
                {
                    throw new Exception($"O destino {i + 1} possui data final anterior à data inicial.");
                }

                // 2️⃣ DtInicial < DtFinal do destino anterior
                if (i > 0)
                {
                    var anterior = destinosOrdenados[i - 1];
                    if (atual.DtInicial.HasValue && anterior.DtFinal.HasValue &&
                        atual.DtInicial.Value < anterior.DtFinal.Value)
                    {
                        throw new Exception($"A data inicial do destino {i + 1} deve ser posterior à data final do destino {i}.");
                    }
                }
            }
        }

        private async Task RemoverDestinosAsync(IEnumerable<RedirecionamentoDestino> destinos)
        {
            foreach (var destino in destinos)
                await _destinoRepository.RemoverAsync(destino.Codigo);
        }

        private static RedirecionamentoDestino MapearDestinoParaEntity(RedirecionamentoDestinoDTO dto)
        {
            return new RedirecionamentoDestino
            {
                Codigo = dto.Codigo,
                UrlDestino = dto.UrlDestino,
                DtInicial = dto.DtInicial,
                DtFinal = dto.DtFinal,
                Ativo = dto.Ativo
            };
        }

        private static RedirecionamentoOrigemDTO MapearOrigemParaDTO(RedirecionamentoOrigem entity)
        {
            return new RedirecionamentoOrigemDTO
            {
                Codigo = entity.Codigo,
                UrlOrigem = entity.UrlOrigem,
                Ativo = entity.Ativo,
                DtRedirecionamento = entity.DtRedirecionamento,
                DtAtualizacao = entity.DtAtualizacao,
                Destinos = entity.Destinos?.Select(d => new RedirecionamentoDestinoDTO
                {
                    Codigo = d.Codigo,
                    RedirecionamentoOrigemId = d.RedirecionamentoOrigemId,
                    UrlDestino = d.UrlDestino,
                    DtInicial = d.DtInicial,
                    DtFinal = d.DtFinal,
                    Ativo = d.Ativo
                }).ToList() ?? new List<RedirecionamentoDestinoDTO>()
            };
        }

        public async Task<(IEnumerable<RedirecionamentoOrigemDTO> Itens, int Total)> ObterPaginadoAsync(int page, int pageSize)
        {
            // 🔹 Chama o repositório (que usa o contexto e faz Skip/Take no banco)
            var (itens, totalItens) = await _repository.ObterPaginadoAsync(page, pageSize);

            // 🔹 Mapeia para DTOs
            var dtos = itens.Select(MapearOrigemParaDTO);

            return (dtos, totalItens);
        }


        #endregion

        #region 🔹 Middleware Helper

        public RedirecionamentoDestinoDTO? SelecionarDestinoValido(RedirecionamentoOrigemDTO origem)
        {
            if (origem?.Destinos == null || !origem.Destinos.Any())
                return null;

            var agora = DateTime.Now;

            return origem.Destinos
                .Where(d =>
                    d.Ativo &&
                    (!d.DtInicial.HasValue || agora >= d.DtInicial.Value) &&
                    (!d.DtFinal.HasValue || agora <= d.DtFinal.Value))
                .OrderBy(d => d.DtInicial ?? DateTime.MinValue)
                .FirstOrDefault();
        }

        #endregion
    }
}
