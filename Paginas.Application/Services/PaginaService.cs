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
        private readonly IBotaoService _botaoService; // ✅ Injetado

        public PaginaService(IPaginaRepository repo, IBotaoService botaoService)
        {
            _repo = repo;
            _botaoService = botaoService;
        }

        public async Task<List<PaginaDTO>> ListarAsync()
        {
            var entidades = await _repo.ListarTodasAsync();
            return entidades.Select(e => e.ToDTO()).ToList();
        }

        public async Task<(List<PaginaDTO> Items, int TotalCount)> ListarPaginadoAsync(int page, int pageSize)
        {
            var (items, total) = await _repo.ObterPaginadoAsync(page, pageSize, apenasRaiz: true);
            var dtos = items.Select(e => e.ToDTO()).ToList();
            return (dtos, total);
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

            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();

            // Agora o código da página está gerado
            // ✅ Adiciona botões usando o BotaoService
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;

                    // ✅ Usa o serviço para criar com ordem automática
                    await _botaoService.CriarAsync(b, pagina.Codigo);
                }
            }

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

                            // ✅ Usa o serviço para criar botões do tópico
                            await _botaoService.CriarAsync(b, sub.Codigo);
                        }
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

            // Atualiza campos principais
            pagina.Atualizar(model.Titulo, model.Conteudo, model.Url, pagina.Tipo);

            // Atualiza banner
            if (!string.IsNullOrWhiteSpace(model.Banner) && model.Banner != pagina.Banner)
                pagina.DefinirBanner(model.Banner);

            // Atualiza status
            if (model.Publicacao) pagina.Publicar(); else pagina.Despublicar();
            if (model.Status) pagina.Ativar(); else pagina.Desativar();

            // ✅ Adiciona novos botões via BotaoService
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;

                    // Evita duplicatas por Nome + Link
                    var existe = pagina.Botoes.Any(bot =>
                        bot.Nome == b.Nome &&
                        bot.Link == b.Link);

                    if (existe) continue;

                    // ✅ Usa o serviço para criar com ordem automática
                    await _botaoService.CriarAsync(b, pagina.Codigo);
                }
            }

            // Adiciona novos tópicos
            if (model.PaginaFilhos != null && model.PaginaFilhos.Any())
            {
                var topicosExistentes = await _repo.ListarFilhosAsync(pagina.Codigo);
                int proximaOrdemTopico = topicosExistentes.Count + 1;

                foreach (var topico in model.PaginaFilhos)
                {
                    var sub = new Pagina(topico.Titulo, topico.Conteudo, topico.Url?.Trim(), TipoPagina.Topico, pagina.Codigo);
                    sub.AtualizarOrdem(proximaOrdemTopico++);

                    if (!string.IsNullOrWhiteSpace(topico.Banner))
                        sub.DefinirBanner(topico.Banner);

                    await _repo.AdicionarAsync(sub);
                    await _repo.SalvarAlteracoesAsync();

                    if (topico.Botoes != null && topico.Botoes.Any())
                    {
                        foreach (var b in topico.Botoes)
                        {
                            if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;

                            // ✅ Usa o serviço para criar botões do tópico
                            await _botaoService.CriarAsync(b, sub.Codigo);
                        }
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

            await _repo.AdicionarAsync(sub);
            await _repo.SalvarAlteracoesAsync();

            // ✅ Usa BotaoService para criar botões
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;

                    await _botaoService.CriarAsync(b, sub.Codigo);
                }
            }
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