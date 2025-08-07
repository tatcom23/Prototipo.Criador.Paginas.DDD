using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories;
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
                Tipo = botao.Tipo,
                Status = botao.Status,
                CdPaginaIntrodutoria = botao.CdPaginaIntrodutoria
            };
        }

        public async Task AtualizarAsync(BotaoDTO dto)
        {
            var botao = await _repository.ObterPorIdAsync(dto.Codigo);
            if (botao == null) return;

            botao.Nome = dto.Nome;
            botao.Link = dto.Link;
            botao.Linha = dto.Linha;
            botao.Coluna = dto.Coluna;
            botao.Tipo = dto.Tipo;
            botao.Status = dto.Status;
            botao.CdPaginaIntrodutoria = dto.CdPaginaIntrodutoria;
            botao.Atualizacao = System.DateTime.Now;

            await _repository.AtualizarAsync(botao);
        }

        public async Task ExcluirAsync(int id)
        {
            await _repository.ExcluirAsync(id);
        }
    }
}
