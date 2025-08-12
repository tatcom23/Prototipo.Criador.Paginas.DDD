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
                Tipo = (int)botao.Tipo, // conversão enum para int
                Status = botao.Status,
                CdPaginaIntrodutoria = botao.CdPaginaIntrodutoria
            };
        }

        public async Task AtualizarAsync(BotaoDTO dto)
        {
            var botao = await _repository.ObterPorIdAsync(dto.Codigo);
            if (botao == null) return;

            // converte int para enum no método Atualizar da entidade
            botao.Atualizar(dto.Nome, dto.Link, (TipoBotao)dto.Tipo, dto.Linha, dto.Coluna);

            if (dto.Status)
                botao.Ativar();
            else
                botao.Desativar();

            await _repository.AtualizarAsync(botao);
        }

        public async Task ExcluirAsync(int id)
        {
            await _repository.RemoverAsync(id);
        }
    }
}
