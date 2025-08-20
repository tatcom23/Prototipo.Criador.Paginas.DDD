using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Domain.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Paginas.Application.Services
{
    public class BotaoService : IBotaoService
    {
        private readonly IBotaoRepository _repository;

        public BotaoService(IBotaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<BotaoDTO> BuscarPorIdAsync(int id)
        {
            var botao = await _repository.ObterPorIdAsync(id);
            if (botao == null) return null;

            return new BotaoDTO
            {
                Codigo = botao.Codigo,
                Nome = botao.Nome,
                Link = botao.Link,
                Linha = botao.Linha,
                Coluna = botao.Coluna,
                Tipo = (int)botao.Tipo,
                Status = botao.Status,
                CdPaginaIntrodutoria = botao.CdPaginaIntrodutoria
            };
        }

        public async Task AtualizarAsync(BotaoDTO dto)
        {
            var botao = await _repository.ObterPorIdAsync(dto.Codigo);
            if (botao == null) return;

            // Atualiza apenas Nome e Link
            botao.Atualizar(dto.Nome, dto.Link, botao.Tipo, botao.Linha, botao.Coluna);

            await _repository.AtualizarAsync(botao);
            await _repository.SalvarAlteracoesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var botao = await _repository.ObterPorIdAsync(id);
            if (botao == null) return;

            await _repository.RemoverAsync(botao.Codigo);
            await _repository.SalvarAlteracoesAsync();
        }

    }
}
