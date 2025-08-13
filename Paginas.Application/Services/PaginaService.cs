using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Domain.Repositories.Interfaces;
using System.Collections.Generic;
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

        // Fluxo corrigido: salva a página, depois adiciona botões (já com PK), idem para filhos
        public async Task CriarAsync(PaginaDTO model, string webRootPath)
        {
            // 1) Cria a página sem botões
            var pagina = new Pagina(
                model.Titulo,
                model.Conteudo,
                model.Url,
                TipoPagina.Principal,
                null
            );

            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAlteracoesAsync(); // Gera pagina.Codigo

            // 2) Adiciona botões agora que a PK existe
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var botaoVm in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(botaoVm.Nome) || string.IsNullOrWhiteSpace(botaoVm.Link))
                        continue;

                    var botao = new Botao(
                        botaoVm.Nome,
                        botaoVm.Link,
                        TipoBotao.Primario,   // ajuste se vier do DTO
                        pagina.Codigo,        // FK correta
                        botaoVm.Linha,
                        botaoVm.Coluna
                    );

                    pagina.AdicionarBotao(botao);
                }

                await _repo.AtualizarAsync(pagina);
                await _repo.SalvarAlteracoesAsync();
            }

            // 3) Cria filhos (tópicos) e aplica o mesmo padrão
            if (model.PaginaFilhos != null && model.PaginaFilhos.Any())
            {
                foreach (var topico in model.PaginaFilhos)
                {
                    // 3.1) Subpágina sem botões
                    var subpagina = new Pagina(
                        topico.Titulo,
                        topico.Conteudo,
                        topico.Url,
                        TipoPagina.Topico,
                        pagina.Codigo // define o pai
                    );

                    await _repo.AdicionarAsync(subpagina);
                    await _repo.SalvarAlteracoesAsync(); // Gera subpagina.Codigo

                    // 3.2) Agora adiciona os botões da subpágina
                    if (topico.Botoes != null && topico.Botoes.Any())
                    {
                        foreach (var b in topico.Botoes)
                        {
                            if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link))
                                continue;

                            var botao = new Botao(
                                b.Nome,
                                b.Link,
                                TipoBotao.Primario,   // ajuste se vier do DTO
                                subpagina.Codigo,     // FK da subpágina
                                b.Linha,
                                b.Coluna
                            );

                            subpagina.AdicionarBotao(botao);
                        }

                        await _repo.AtualizarAsync(subpagina);
                        await _repo.SalvarAlteracoesAsync();
                    }
                }
            }
        }

        public async Task AtualizarAsync(int id, PaginaDTO model, string webRootPath)
        {
            var pagina = await _repo.ObterPorIdAsync(id);
            if (pagina == null) return;

            pagina.Atualizar(model.Titulo, model.Conteudo, model.Url, pagina.Tipo);

            // Se chegarem botões novos no update, adiciona (PK do pai já existe)
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link))
                        continue;

                    var botao = new Botao(
                        b.Nome,
                        b.Link,
                        TipoBotao.Primario,
                        pagina.Codigo,  // FK do pai já existente
                        b.Linha,
                        b.Coluna
                    );

                    pagina.AdicionarBotao(botao);
                }
            }

            await _repo.AtualizarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task CriarComPaiAsync(PaginaDTO model, int cdPai)
        {
            // Pode calcular ordem se necessário (já tem método AtualizarOrdem no agregado)
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

            // Se for página principal, remove filhos antes
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

            // Reordena irmãos se a excluída era filha
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
