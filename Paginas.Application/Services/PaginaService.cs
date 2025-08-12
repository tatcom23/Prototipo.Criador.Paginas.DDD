using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paginas.Application.Services
{
    public class PaginaService : IPaginaService
    {
        private readonly IPaginaRepository _repo;

        public PaginaService(IPaginaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Pagina>> ListarAsync()
        {
            return await _repo.ListarTodasAsync();
        }

        public async Task<Pagina> BuscarPorIdAsync(int id)
        {
            return await _repo.ObterPorIdAsync(id);
        }

        public async Task CriarAsync(PaginaDTO model, string webRootPath)
        {
            var pagina = new Pagina(
                model.Titulo,
                model.Conteudo,
                model.Url,
                TipoPagina.Principal,
                null
            );

            if (model.Botoes != null)
            {
                foreach (var botaoVm in model.Botoes)
                {
                    if (!string.IsNullOrWhiteSpace(botaoVm.Nome) && !string.IsNullOrWhiteSpace(botaoVm.Link))
                    {
                        var botao = new Botao(
                            botaoVm.Nome,
                            botaoVm.Link,
                            TipoBotao.Primario,  // <-- Usar valor válido do enum
                            pagina.Codigo,
                            botaoVm.Linha,
                            botaoVm.Coluna
                        );
                        pagina.AdicionarBotao(botao);
                    }
                }
            }

            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();

            if (model.PaginaFilhos != null && model.PaginaFilhos.Any())
            {
                int ordemAtual = 1;

                foreach (var topico in model.PaginaFilhos)
                {
                    var subpagina = new Pagina(
                        topico.Titulo,
                        topico.Conteudo,
                        topico.Url,
                        TipoPagina.Topico,
                        pagina.Codigo
                    );

                    if (topico.Botoes != null)
                    {
                        foreach (var b in topico.Botoes)
                        {
                            if (!string.IsNullOrWhiteSpace(b.Nome) && !string.IsNullOrWhiteSpace(b.Link))
                            {
                                var botao = new Botao(
                                    b.Nome,
                                    b.Link,
                                    TipoBotao.Primario,  // <-- Usar valor válido do enum
                                    subpagina.Codigo,
                                    b.Linha,
                                    b.Coluna
                                );
                                subpagina.AdicionarBotao(botao);
                            }
                        }
                    }

                    await _repo.AdicionarAsync(subpagina);
                    ordemAtual++;
                }

                await _repo.SalvarAlteracoesAsync();
            }
        }

        public async Task AtualizarAsync(int id, PaginaDTO model, string webRootPath)
        {
            var pagina = await _repo.ObterPorIdAsync(id);
            if (pagina == null) return;

            pagina.Atualizar(model.Titulo, model.Conteudo, model.Url, pagina.Tipo);

            if (model.Botoes != null)
            {
                foreach (var b in model.Botoes)
                {
                    if (!string.IsNullOrWhiteSpace(b.Nome) && !string.IsNullOrWhiteSpace(b.Link))
                    {
                        var botao = new Botao(
                            b.Nome,
                            b.Link,
                            TipoBotao.Primario,  // <-- Usar valor válido do enum
                            pagina.Codigo,
                            b.Linha,
                            b.Coluna
                        );
                        pagina.AdicionarBotao(botao);
                    }
                }
            }

            await _repo.AtualizarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task CriarComPaiAsync(PaginaDTO model, int cdPai)
        {
            var paginasFilhas = await _repo.ListarFilhosAsync(cdPai);
            int ordemAtualMax = paginasFilhas.Any() ? paginasFilhas.Max(p => p.Ordem) : 0;

            var pagina = new Pagina(
                model.Titulo,
                model.Conteudo,
                model.Url,
                TipoPagina.Topico,
                cdPai
            );

            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var pagina = await _repo.ObterPorIdAsync(id);
            if (pagina == null) return;

            var cdPai = pagina.CdPai;

            if (cdPai == null)
            {
                var topicos = (await _repo.ListarFilhosAsync(pagina.Codigo)).ToList();

                foreach (var topico in topicos)
                {
                    topico.Botoes?.Clear();
                    await _repo.RemoverAsync(topico);
                }
            }

            pagina.Botoes?.Clear();
            await _repo.RemoverAsync(pagina);
            await _repo.SalvarAlteracoesAsync();

            if (cdPai != null)
            {
                var topicos = (await _repo.ListarFilhosAsync(cdPai.Value)).OrderBy(p => p.Ordem).ToList();

                for (int i = 0; i < topicos.Count; i++)
                {
                    topicos[i].AtualizarOrdem(i + 1);
                }

                await _repo.SalvarAlteracoesAsync();
            }

        }

        public async Task AtualizarOrdemAsync(Pagina a, Pagina b)
        {
            await _repo.AtualizarAsync(a);
            await _repo.AtualizarAsync(b);
            await _repo.SalvarAlteracoesAsync();
        }
    }
}
