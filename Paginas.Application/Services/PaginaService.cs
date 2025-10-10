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
        private readonly IBotaoService _botaoService;
        private readonly ICarrosselService _carrosselService;
        private readonly ICarrosselImagemService _carrosselImagemService;

        public PaginaService(
            IPaginaRepository repo,
            IBotaoService botaoService,
            ICarrosselService carrosselService,
            ICarrosselImagemService carrosselImagemService)
        {
            _repo = repo;
            _botaoService = botaoService;
            _carrosselService = carrosselService;
            _carrosselImagemService = carrosselImagemService;
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
            if (entidade == null) return null;

            var dto = entidade.ToDTO();

            // Carrega carrosséis e imagens
            var carrosseis = await _carrosselService.ListarPorPaginaAsync(entidade.Codigo);
            foreach (var c in carrosseis)
            {
                c.Imagens = await _carrosselImagemService.ListarPorCarrosselAsync(c.Codigo);
            }

            dto.Carrosseis = carrosseis;
            return dto;
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

            // Adiciona botões
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;
                    await _botaoService.CriarAsync(b, pagina.Codigo);
                }
            }

            // Adiciona carrosséis e imagens
            if (model.Carrosseis != null && model.Carrosseis.Any())
            {
                foreach (var c in model.Carrosseis)
                {
                    // ✅ Correção: passar o codigo da página como segundo argumento
                    await _carrosselService.CriarAsync(c, pagina.Codigo);

                    if (c.Imagens != null && c.Imagens.Any())
                    {
                        foreach (var img in c.Imagens)
                        {
                            img.CdCarrossel = c.Codigo;
                            await _carrosselImagemService.CriarAsync(img);
                        }
                    }
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
            pagina.Atualizar(model.Titulo, model.Conteudo, model.Url, pagina.Tipo);

            if (!string.IsNullOrWhiteSpace(model.Banner) && model.Banner != pagina.Banner)
                pagina.DefinirBanner(model.Banner);

            if (model.Publicacao) pagina.Publicar(); else pagina.Despublicar();
            if (model.Status) pagina.Ativar(); else pagina.Desativar();

            // Atualiza botões
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link)) continue;
                    var existe = pagina.Botoes.Any(bot => bot.Nome == b.Nome && bot.Link == b.Link);
                    if (!existe) await _botaoService.CriarAsync(b, pagina.Codigo);
                }
            }

            // Atualiza carrosséis e imagens
            if (model.Carrosseis != null && model.Carrosseis.Any())
            {
                foreach (var c in model.Carrosseis)
                {
                    if (c.Codigo == 0)
                    {
                        // ✅ Correção: passar o codigo da página como segundo argumento
                        await _carrosselService.CriarAsync(c, pagina.Codigo);
                    }
                    else
                    {
                        await _carrosselService.AtualizarAsync(c.Codigo, c);
                    }

                    if (c.Imagens != null && c.Imagens.Any())
                    {
                        foreach (var img in c.Imagens)
                        {
                            if (img.Codigo == 0)
                            {
                                img.CdCarrossel = c.Codigo;
                                await _carrosselImagemService.CriarAsync(img);
                            }
                            else
                            {
                                await _carrosselImagemService.AtualizarAsync(img.Codigo, img);
                            }
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

            // --- Exclui carrosséis e imagens associados ---
            var carrosseis = await _carrosselService.ListarPorPaginaAsync(pagina.Codigo);
            foreach (var c in carrosseis)
            {
                var imagens = await _carrosselImagemService.ListarPorCarrosselAsync(c.Codigo);
                foreach (var img in imagens)
                {
                    await _carrosselImagemService.ExcluirAsync(img.Codigo);
                }

                await _carrosselService.ExcluirAsync(c.Codigo);
            }

            // --- Exclui tópicos filhos e seus botões ---
            if (pagina.CdPai == null)
            {
                var topicos = await _repo.ListarFilhosAsync(pagina.Codigo);
                foreach (var t in topicos)
                {
                    if (t.Botoes != null)
                    {
                        foreach (var b in t.Botoes)
                        {
                            await _botaoService.ExcluirAsync(b.Codigo);
                        }
                    }

                    await _repo.RemoverAsync(t);
                }
            }

            // --- Exclui botões da página principal ---
            if (pagina.Botoes != null)
            {
                foreach (var b in pagina.Botoes)
                {
                    await _botaoService.ExcluirAsync(b.Codigo);
                }
            }

            // --- Remove a própria página ---
            await _repo.RemoverAsync(pagina);

            await _repo.SalvarAlteracoesAsync();
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
        public async Task<DashboardViewModel> ObterDadosDashboardAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var todasPaginas = await _repo.ListarTodasAsync();

            // Filtro base: apenas páginas principais e ativas
            var paginasAtivas = todasPaginas
                .Where(p => p.Status && p.CdPai == null)
                .ToList();

            // Se o usuário definiu um período, aplica o filtro
            if (dataInicio.HasValue && dataFim.HasValue)
            {
                paginasAtivas = paginasAtivas
                    .Where(p => p.Criacao.Date >= dataInicio.Value.Date && p.Criacao.Date <= dataFim.Value.Date)
                    .ToList();
            }

            // Total de páginas principais ativas no período
            int totalPaginasAtivas = paginasAtivas.Count;

            // Tabela
            var tabela = paginasAtivas
                .Select(p => new TabelaItem
                {
                    Titulo = p.Titulo,
                    Criacao = p.Criacao,
                    Atualizacao = p.Atualizacao,
                    QuantidadeTopicos = p.PaginaFilhos?.Count() ?? 0
                })
                .OrderByDescending(p => p.Criacao)
                .ToList();

            // Gráfico: quantidade de páginas criadas por mês dentro do período
            var grafico = paginasAtivas
                .GroupBy(p => new { p.Criacao.Year, p.Criacao.Month })
                .Select(g => new GraficoItem
                {
                    MesAno = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Quantidade = g.Count()
                })
                .OrderBy(g => g.MesAno)
                .ToList();

            return new DashboardViewModel
            {
                TotalPaginasAtivas = totalPaginasAtivas,
                Tabela = tabela,
                Grafico = grafico
            };
        }

    }
}
