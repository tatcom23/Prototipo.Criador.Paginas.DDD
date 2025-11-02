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
    public class RedirecionamentoDestinoService : IRedirecionamentoDestinoService
    {
        private readonly IRedirecionamentoDestinoRepository _repository;

        public RedirecionamentoDestinoService(IRedirecionamentoDestinoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RedirecionamentoDestinoDTO>> ObterPorOrigemAsync(int origemId)
        {
            var entities = await _repository.ObterPorOrigemAsync(origemId);

            return entities.Select(d => new RedirecionamentoDestinoDTO
            {
                Codigo = d.Codigo,
                RedirecionamentoOrigemId = d.RedirecionamentoOrigemId,
                UrlDestino = d.UrlDestino,
                DtInicial = d.DtInicial,
                DtFinal = d.DtFinal,
                Ativo = d.Ativo
            });
        }

        public async Task<RedirecionamentoDestinoDTO?> ObterPorIdAsync(int id)
        {
            var entity = await _repository.ObterPorIdAsync(id);
            if (entity == null) return null;

            return new RedirecionamentoDestinoDTO
            {
                Codigo = entity.Codigo,
                RedirecionamentoOrigemId = entity.RedirecionamentoOrigemId,
                UrlDestino = entity.UrlDestino,
                DtInicial = entity.DtInicial,
                DtFinal = entity.DtFinal,
                Ativo = entity.Ativo
            };
        }

        public async Task AdicionarAsync(RedirecionamentoDestinoDTO dto)
        {
            var entity = new RedirecionamentoDestino
            {
                RedirecionamentoOrigemId = dto.RedirecionamentoOrigemId,
                UrlDestino = dto.UrlDestino,
                DtInicial = dto.DtInicial,
                DtFinal = dto.DtFinal,
                Ativo = dto.Ativo
            };

            await _repository.AdicionarAsync(entity);
        }

        public async Task AtualizarAsync(RedirecionamentoDestinoDTO dto)
        {
            var entityExistente = await _repository.ObterPorIdAsync(dto.Codigo);
            if (entityExistente == null)
                throw new Exception("Destino não encontrado.");

            entityExistente.UrlDestino = dto.UrlDestino;
            entityExistente.DtInicial = dto.DtInicial;
            entityExistente.DtFinal = dto.DtFinal;
            entityExistente.Ativo = dto.Ativo;

            await _repository.AtualizarAsync(entityExistente);
        }

        public async Task RemoverAsync(int id)
        {
            await _repository.RemoverAsync(id);
        }
    }
}
