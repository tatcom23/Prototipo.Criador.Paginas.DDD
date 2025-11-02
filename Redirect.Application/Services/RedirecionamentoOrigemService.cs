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

        public async Task<RedirecionamentoOrigemDTO?> ObterPorUrlOrigemAsync(string urlOrigem)
        {
            var entity = await _repository.ObterComDestinosAsync(urlOrigem);
            if (entity == null) return null;

            return new RedirecionamentoOrigemDTO
            {
                Codigo = entity.Codigo,
                UrlOrigem = entity.UrlOrigem,
                Ativo = entity.Ativo,
                DtRedirecionamento = entity.DtRedirecionamento,
                DtAtualizacao = entity.DtAtualizacao,
                Destinos = entity.Destinos.Select(d => new RedirecionamentoDestinoDTO
                {
                    Codigo = d.Codigo,
                    RedirecionamentoOrigemId = d.RedirecionamentoOrigemId,
                    UrlDestino = d.UrlDestino,
                    DtInicial = d.DtInicial,
                    DtFinal = d.DtFinal,
                    Ativo = d.Ativo
                }).ToList()
            };
        }

        public async Task<RedirecionamentoOrigemDTO?> ObterPorIdAsync(int id)
        {
            var entity = await _repository.ObterPorIdAsync(id);
            if (entity == null) return null;

            return new RedirecionamentoOrigemDTO
            {
                Codigo = entity.Codigo,
                UrlOrigem = entity.UrlOrigem,
                Ativo = entity.Ativo,
                DtRedirecionamento = entity.DtRedirecionamento,
                DtAtualizacao = entity.DtAtualizacao,
                Destinos = entity.Destinos.Select(d => new RedirecionamentoDestinoDTO
                {
                    Codigo = d.Codigo,
                    RedirecionamentoOrigemId = d.RedirecionamentoOrigemId,
                    UrlDestino = d.UrlDestino,
                    DtInicial = d.DtInicial,
                    DtFinal = d.DtFinal,
                    Ativo = d.Ativo
                }).ToList()
            };
        }
        public async Task<IEnumerable<RedirecionamentoOrigemDTO>> ObterTodosAsync()
        {
            var entities = await _repository.ObterTodosAsync();

            return entities.Select(r => new RedirecionamentoOrigemDTO
            {
                Codigo = r.Codigo,
                UrlOrigem = r.UrlOrigem,
                Ativo = r.Ativo,
                DtRedirecionamento = r.DtRedirecionamento,
                DtAtualizacao = r.DtAtualizacao,
                Destinos = r.Destinos.Select(d => new RedirecionamentoDestinoDTO
                {
                    Codigo = d.Codigo,
                    RedirecionamentoOrigemId = d.RedirecionamentoOrigemId,
                    UrlDestino = d.UrlDestino,
                    DtInicial = d.DtInicial,
                    DtFinal = d.DtFinal,
                    Ativo = d.Ativo
                }).ToList()
            });
        }

        public async Task AdicionarAsync(RedirecionamentoOrigemDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = new RedirecionamentoOrigem
            {
                UrlOrigem = dto.UrlOrigem,
                Ativo = true,
                DtRedirecionamento = dto.DtRedirecionamento ?? DateTime.Now,
                DtAtualizacao = DateTime.Now,
                Destinos = new List<RedirecionamentoDestino>()
            };

            if (dto.Destinos != null && dto.Destinos.Any())
            {
                foreach (var destinoDto in dto.Destinos)
                {
                    var destino = new RedirecionamentoDestino
                    {
                        UrlDestino = destinoDto.UrlDestino,
                        DtInicial = destinoDto.DtInicial,
                        DtFinal = destinoDto.DtFinal,
                        Ativo = true
                    };
                    entity.Destinos.Add(destino);
                }
            }

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

            // Atualiza ou adiciona destinos
            foreach (var destinoDto in destinosDto)
            {
                var destinoExistente = entityExistente.Destinos
                    .FirstOrDefault(d => d.Codigo == destinoDto.Codigo);

                if (destinoExistente != null)
                {
                    destinoExistente.UrlDestino = destinoDto.UrlDestino;
                    destinoExistente.DtInicial = destinoDto.DtInicial;
                    destinoExistente.DtFinal = destinoDto.DtFinal;
                    destinoExistente.Ativo = destinoDto.Ativo;
                }
                else
                {
                    entityExistente.Destinos.Add(new RedirecionamentoDestino
                    {
                        UrlDestino = destinoDto.UrlDestino,
                        DtInicial = destinoDto.DtInicial,
                        DtFinal = destinoDto.DtFinal,
                        Ativo = destinoDto.Ativo
                    });
                }
            }

            // 🔹 Remove destinos que foram excluídos no formulário
            var destinosParaRemover = entityExistente.Destinos
                .Where(d => !destinosDto.Any(dto => dto.Codigo == d.Codigo))
                .ToList();

            foreach (var destino in destinosParaRemover)
            {
                // Remove do repositório de destinos para evitar conflito de FK
                await _destinoRepository.RemoverAsync(destino.Codigo);
            }

            await _repository.AtualizarAsync(entityExistente);
        }

        public async Task RemoverAsync(int id)
        {
            var entity = await _repository.ObterPorIdAsync(id);
            if (entity == null)
                throw new Exception("Redirecionamento não encontrado.");

            // 🔹 Remove todos os destinos associados no banco
            if (entity.Destinos != null && entity.Destinos.Any())
            {
                foreach (var destino in entity.Destinos.ToList())
                {
                    await _destinoRepository.RemoverAsync(destino.Codigo);
                }
            }

            // 🔹 Remove a origem
            await _repository.RemoverAsync(entity.Codigo);
        }

        // 🔹 Método usado pelo Middleware
        public RedirecionamentoDestinoDTO? SelecionarDestinoValido(RedirecionamentoOrigemDTO origem)
        {
            if (origem == null || origem.Destinos == null || !origem.Destinos.Any())
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
    }
}
