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
    public class RedirectURLService : IRedirectURLService
    {
        private readonly IRedirectURLRepository _repository;

        public RedirectURLService(IRedirectURLRepository repository)
        {
            _repository = repository;
        }

        public async Task<RedirectURLDTO?> ObterPorUrlAntigaAsync(string urlAntiga)
        {
            var entity = await _repository.ObterPorUrlAntigaAsync(urlAntiga);
            if (entity == null) return null;

            return new RedirectURLDTO
            {
                Codigo = entity.Codigo,
                DtRedirectUrl = entity.DtRedirectUrl,
                DtAtualizacao = entity.DtAtualizacao,
                UrlAntiga = entity.UrlAntiga,
                UrlNova = entity.UrlNova,
                Ativo = entity.Ativo
            };
        }

        public async Task<IEnumerable<RedirectURLDTO>> ObterTodosAsync()
        {
            var entities = await _repository.ObterTodosAsync();
            return entities.Select(r => new RedirectURLDTO
            {
                Codigo = r.Codigo,
                DtRedirectUrl = r.DtRedirectUrl,
                DtAtualizacao = r.DtAtualizacao,
                UrlAntiga = r.UrlAntiga,
                UrlNova = r.UrlNova,
                Ativo = r.Ativo
            });
        }

        public async Task AdicionarAsync(RedirectURLDTO dto)
        {
            var entity = new RedirectURL
            {
                UrlAntiga = dto.UrlAntiga,
                UrlNova = dto.UrlNova,
                Ativo = dto.Ativo
            };
            await _repository.AdicionarAsync(entity);
        }

        public async Task AtualizarAsync(RedirectURLDTO dto)
        {
            var entityExistente = await _repository.ObterPorIdAsync(dto.Codigo);
            if (entityExistente == null) throw new Exception("Redirecionamento não encontrado.");

            // Atualiza apenas os campos editáveis
            entityExistente.UrlAntiga = dto.UrlAntiga;
            entityExistente.UrlNova = dto.UrlNova;
            entityExistente.Ativo = true;

            await _repository.AtualizarAsync(entityExistente);
        }

        public async Task RemoverAsync(int id)
        {
            await _repository.RemoverAsync(id);
        }
    }
}
