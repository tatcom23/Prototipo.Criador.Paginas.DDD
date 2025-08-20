using Paginas.Application.DTOs;
using Paginas.Application.Mappers;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Domain.Repositories.Interfaces;
using System;
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

        public async Task<List<PaginaDTO>> ListarAsync()
        {
            var entidades = await _repo.ListarTodasAsync();
            return entidades.Select(e => e.ToDTO()).ToList();
        }

        public async Task<List<PaginaDTO>> ListarFilhosAsync(int cdPai)
        {
            var filhos = await _repo.ListarFilhosAsync(cdPai);
            return filhos.Select(e => e.ToDTO()).ToList();
        }

        public async Task<PaginaDTO> BuscarPorIdAsync(int id)
        {
            var entidade = await _repo.ObterPorIdAsync(id);
            return entidade?.ToDTO();
        }

        public async Task CriarAsync(PaginaDTO model, string webRootPath)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            model.Url = model.Url?.Trim();

            var tipo = model.CdPai.HasValue ? TipoPagina.Topico : TipoPagina.Principal;

            var pagina = new Pagina(model.Titulo, model.Conteudo, model.Url, tipo, model.CdPai);

            if (!string.IsNullOrWhiteSpace(model.Banner))
                pagina.DefinirBanner(model.Banner);

            // **Adicionar botões da página principal e tópicos**
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;
                    var botao = new Botao(b.Nome, b.Link, TipoBotao.Primario, pagina.Codigo, b.Linha, b.Coluna);
                    pagina.AdicionarBotao(botao);
                }
            }

            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();

            // Adiciona tópicos (filhos)
            if (model.PaginaFilhos != null && model.PaginaFilhos.Any())
            {
                int ordem = 1;
                foreach (var topico in model.PaginaFilhos)
                {
                    var sub = new Pagina(topico.Titulo, topico.Conteudo, topico.Url?.Trim(), TipoPagina.Topico, pagina.Codigo);
                    sub.AtualizarOrdem(ordem++);

                    if (!string.IsNullOrWhiteSpace(topico.Banner))
                        sub.DefinirBanner(topico.Banner);

                    await _repo.AdicionarAsync(sub);
                    await _repo.SalvarAlteracoesAsync();

                    if (topico.Botoes != null && topico.Botoes.Any())
                    {
                        foreach (var b in topico.Botoes)
                        {
                            if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;
                            var botao = new Botao(b.Nome, b.Link, TipoBotao.Primario, sub.Codigo, b.Linha, b.Coluna);
                            sub.AdicionarBotao(botao);
                        }
                        await _repo.AtualizarAsync(sub);
                        await _repo.SalvarAlteracoesAsync();
                    }
                }
            }
        }

        public async Task AtualizarAsync(int id, PaginaDTO model, string webRootPath)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var pagina = await _repo.ObterPorIdAsync(id);
            if (pagina == null) return;

            model.Url = model.Url?.Trim();
            var tipo = pagina.Tipo;

            pagina.Atualizar(model.Titulo, model.Conteudo, model.Url, tipo);

            if (!string.IsNullOrWhiteSpace(model.Banner) && model.Banner != pagina.Banner)
                pagina.DefinirBanner(model.Banner);

            if (model.Publicacao) pagina.Publicar(); else pagina.Despublicar();
            if (model.Status) pagina.Ativar(); else pagina.Desativar();

            // **NOVO:** só adiciona botões se for tópico, não altera botões existentes da página principal
            if (tipo == TipoPagina.Topico && model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;
                    var botao = new Botao(b.Nome, b.Link, TipoBotao.Primario, pagina.Codigo, b.Linha, b.Coluna);
                    pagina.AdicionarBotao(botao);
                }
            }

            // Adiciona novos tópicos
            if (model.PaginaFilhos != null && model.PaginaFilhos.Any())
            {
                int ordem = (await _repo.ListarFilhosAsync(pagina.Codigo)).Count + 1;
                foreach (var topico in model.PaginaFilhos)
                {
                    var sub = new Pagina(topico.Titulo, topico.Conteudo, topico.Url?.Trim(), TipoPagina.Topico, pagina.Codigo);
                    sub.AtualizarOrdem(ordem++);

                    if (!string.IsNullOrWhiteSpace(topico.Banner))
                        sub.DefinirBanner(topico.Banner);

                    await _repo.AdicionarAsync(sub);
                    await _repo.SalvarAlteracoesAsync();

                    if (topico.Botoes != null && topico.Botoes.Any())
                    {
                        foreach (var b in topico.Botoes)
                        {
                            if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;
                            var botao = new Botao(b.Nome, b.Link, TipoBotao.Primario, sub.Codigo, b.Linha, b.Coluna);
                            sub.AdicionarBotao(botao);
                        }
                        await _repo.AtualizarAsync(sub);
                        await _repo.SalvarAlteracoesAsync();
                    }
                }
            }

            await _repo.AtualizarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task CriarComPaiAsync(PaginaDTO model, int cdPai)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var sub = new Pagina(model.Titulo, model.Conteudo, model.Url?.Trim(), TipoPagina.Topico, cdPai);

            // Botões de tópicos
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;
                    var botao = new Botao(b.Nome, b.Link, TipoBotao.Primario, sub.Codigo, b.Linha, b.Coluna);
                    sub.AdicionarBotao(botao);
                }
            }

            await _repo.AdicionarAsync(sub);
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
                foreach (var t in topicos)
                {
                    t.Botoes?.Clear();
                    await _repo.RemoverAsync(t);
                }
            }

            pagina.Botoes?.Clear();
            await _repo.RemoverAsync(pagina);
            await _repo.SalvarAlteracoesAsync();

            if (cdPai != null)
            {
                var topicos = (await _repo.ListarFilhosAsync(cdPai.Value)).OrderBy(p => p.Ordem).ToList();
                for (int i = 0; i < topicos.Count; i++)
                    topicos[i].AtualizarOrdem(i + 1);
                await _repo.SalvarAlteracoesAsync();
            }
        }

        public async Task AtualizarOrdemAsync(int idA, int idB)
        {
            var a = await _repo.ObterPorIdAsync(idA);
            var b = await _repo.ObterPorIdAsync(idB);
            if (a == null || b == null) return;

            var ordemA = a.Ordem;
            var ordemB = b.Ordem;

            a.AtualizarOrdem(ordemB);
            b.AtualizarOrdem(ordemA);

            await _repo.AtualizarAsync(a);
            await _repo.AtualizarAsync(b);
            await _repo.SalvarAlteracoesAsync();
        }
    }
}
