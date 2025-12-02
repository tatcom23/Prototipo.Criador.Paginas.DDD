using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Domain.Repositories.Interfaces;
using System;
using System.Linq;
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

        public async Task CriarAsync(BotaoDTO dto, int cdPaginaIntrodutoria)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var tipo = (TipoBotao)dto.Tipo;
            var maxOrdem = await _repository.ObterMaxOrdemPorPaginaAsync(cdPaginaIntrodutoria);
            var novaOrdem = maxOrdem + 1;

            var botao = new Botao(
                nome: dto.Nome,
                link: dto.Link,
                tipo,
                cdPaginaIntrodutoria: cdPaginaIntrodutoria,
                ordem: novaOrdem
            );

            await _repository.AdicionarAsync(botao);
            await _repository.SalvarAlteracoesAsync();

            // Preenche o DTO com os dados gerados
            dto.Codigo = botao.Codigo;
            dto.Ordem = botao.Ordem;
            dto.CdPaginaIntrodutoria = botao.CdPaginaIntrodutoria;
            dto.Criacao = botao.Criacao;
            dto.Atualizacao = botao.Atualizacao;
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
                Tipo = (int)botao.Tipo,
                CdPaginaIntrodutoria = botao.CdPaginaIntrodutoria,
                Ordem = botao.Ordem,
                Status = botao.Status,
                Criacao = botao.Criacao,
                Atualizacao = botao.Atualizacao
            };
        }

        public async Task AtualizarAsync(BotaoDTO dto)
        {
            var botao = await _repository.ObterPorIdAsync(dto.Codigo);
            if (botao == null) return;

            botao.Atualizar(
                nome: dto.Nome,
                link: dto.Link,
                tipo: botao.Tipo,
                ordem: dto.Ordem
            );

            await _repository.AtualizarAsync(botao);
            await _repository.SalvarAlteracoesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var botao = await _repository.ObterPorIdAsync(id);
            if (botao == null) return;

            var cdPagina = botao.CdPaginaIntrodutoria;

            await _repository.RemoverAsync(botao.Codigo);
            await _repository.SalvarAlteracoesAsync();

            // Reordena os botões da página
            await ReordenarBotoesPaginaAsync(cdPagina);
        }

        private async Task ReordenarBotoesPaginaAsync(int cdPagina)
        {
            var botoes = await _repository.ListarPorPaginaAsync(cdPagina);
            var ordenados = botoes.OrderBy(b => b.Ordem).ToList();

            for (int i = 0; i < ordenados.Count; i++)
            {
                var botao = ordenados[i];
                if (botao.Ordem != i + 1)
                {
                    botao.Atualizar(botao.Nome, botao.Link, botao.Tipo, i + 1);
                    await _repository.AtualizarAsync(botao);
                }
            }

            await _repository.SalvarAlteracoesAsync();
        }

        public async Task AtualizarOrdemAsync(int idA, int idB)
        {
            if (idA == idB) return;

            var botaoA = await _repository.ObterPorIdAsync(idA);
            var botaoB = await _repository.ObterPorIdAsync(idB);

            if (botaoA == null || botaoB == null)
                throw new ArgumentException("Um ou ambos os botões não foram encontrados.");

            var ordemTemp = botaoA.Ordem;

            botaoA.Atualizar(botaoA.Nome, botaoA.Link, botaoA.Tipo, botaoB.Ordem);
            botaoB.Atualizar(botaoB.Nome, botaoB.Link, botaoB.Tipo, ordemTemp);

            await _repository.AtualizarAsync(botaoA);
            await _repository.AtualizarAsync(botaoB);
            await _repository.SalvarAlteracoesAsync();
        }

        public async Task AtualizarOrdemIndividualAsync(int botaoId, int novaOrdem)
        {
            var botao = await _repository.ObterPorIdAsync(botaoId);
            if (botao == null) return;

            botao.Atualizar(botao.Nome, botao.Link, botao.Tipo, novaOrdem);
            await _repository.AtualizarAsync(botao);
            await _repository.SalvarAlteracoesAsync();
        }
    }
}