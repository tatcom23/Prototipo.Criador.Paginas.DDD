// Application -> Services -> PaginaService.cs
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

        // Retorna DTOs
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

        // Criação: constrói a entidade com construtor do domínio; salva; depois adiciona botões/filhos
        public async Task CriarAsync(PaginaDTO model, string webRootPath)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Normaliza a URL para evitar espaços acidentais
            model.Url = model.Url?.Trim();

            // Se CdPai tiver valor => Topico; senão => Principal (NÃO depende do valor vindo do form)
            var tipo = model.CdPai.HasValue ? TipoPagina.Topico : TipoPagina.Principal;

            var pagina = new Pagina(
                model.Titulo,
                model.Conteudo,
                model.Url,
                tipo,
                model.CdPai
            );

            // Define banner se foi informado pela camada de apresentação (controller já coloca o caminho em model.Banner)
            if (!string.IsNullOrWhiteSpace(model.Banner))
            {
                pagina.DefinirBanner(model.Banner);
            }

            // Persistir a página para gerar PK
            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAlteracoesAsync(); // garante pagina.Codigo

            // Botões da página (após PK existir)
            if (model.Botoes != null && model.Botoes.Any())
            {
                foreach (var b in model.Botoes)
                {
                    if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link))
                        continue;

                    var botao = new Botao(
                        b.Nome,
                        b.Link,
                        TipoBotao.Primario,     // ajuste se seu DTO trouxer o tipo
                        pagina.Codigo,
                        b.Linha,
                        b.Coluna
                    );

                    pagina.AdicionarBotao(botao);
                }

                await _repo.AtualizarAsync(pagina);
                await _repo.SalvarAlteracoesAsync();
            }

            // Filhos (tópicos)
            if (model.PaginaFilhos != null && model.PaginaFilhos.Any())
            {
                foreach (var topico in model.PaginaFilhos)
                {
                    // garante url do tópico normalizada
                    var topicoUrl = topico.Url?.Trim();

                    // define subpágina sempre como Topico
                    var sub = new Pagina(
                        topico.Titulo,
                        topico.Conteudo,
                        topicoUrl,
                        TipoPagina.Topico,
                        pagina.Codigo
                    );

                    // define banner do tópico se fornecido
                    if (!string.IsNullOrWhiteSpace(topico.Banner))
                    {
                        sub.DefinirBanner(topico.Banner);
                    }

                    await _repo.AdicionarAsync(sub);
                    await _repo.SalvarAlteracoesAsync();

                    // Botões do subtopico (após PK do sub existir)
                    if (topico.Botoes != null && topico.Botoes.Any())
                    {
                        foreach (var b in topico.Botoes)
                        {
                            if (string.IsNullOrWhiteSpace(b.Nome) || string.IsNullOrWhiteSpace(b.Link))
                                continue;

                            var botao = new Botao(
                                b.Nome,
                                b.Link,
                                TipoBotao.Primario,
                                sub.Codigo,
                                b.Linha,
                                b.Coluna
                            );

                            sub.AdicionarBotao(botao);
                        }

                        await _repo.AtualizarAsync(sub);
                        await _repo.SalvarAlteracoesAsync();
                    }
                }
            }
        }

        // Atualização: busca entidade e usa métodos do domínio
        public async Task AtualizarAsync(int id, PaginaDTO model, string webRootPath)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var pagina = await _repo.ObterPorIdAsync(id);
            if (pagina == null) return;

            // Normaliza a URL no update também
            model.Url = model.Url?.Trim();

            // Atualiza campos principais; mantém o tipo atual da entidade
            var tipo = pagina.Tipo; // manter tipo atual; se quiser permitir mudança, use (TipoPagina)model.Tipo
            pagina.Atualizar(model.Titulo, model.Conteudo, model.Url, tipo);

            // Atualiza banner se informado (preserva existente caso model.Banner seja null/empty)
            if (!string.IsNullOrWhiteSpace(model.Banner) && model.Banner != pagina.Banner)
            {
                pagina.DefinirBanner(model.Banner);
            }

            // Publicação/Status através de métodos do domínio
            if (model.Publicacao) pagina.Publicar(); else pagina.Despublicar();
            if (model.Status) pagina.Ativar(); else pagina.Desativar();

            // Novos botões (se enviados no update)
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
                        pagina.Codigo,
                        b.Linha,
                        b.Coluna
                    );

                    pagina.AdicionarBotao(botao);
                }
            }

            await _repo.AtualizarAsync(pagina);
            await _repo.SalvarAlteracoesAsync();
        }

        // Cria uma subpágina (tópico) com pai informado
        public async Task CriarComPaiAsync(PaginaDTO model, int cdPai)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var sub = new Pagina(
                model.Titulo,
                model.Conteudo,
                model.Url?.Trim(),
                TipoPagina.Topico,
                cdPai
            );

            if (!string.IsNullOrWhiteSpace(model.Banner))
            {
                sub.DefinirBanner(model.Banner);
            }

            await _repo.AdicionarAsync(sub);
            await _repo.SalvarAlteracoesAsync();

            // Se vierem botões para o tópico, adiciona-os
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
                        sub.Codigo,
                        b.Linha,
                        b.Coluna
                    );

                    sub.AdicionarBotao(botao);
                }

                await _repo.AtualizarAsync(sub);
                await _repo.SalvarAlteracoesAsync();
            }
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

                foreach (var t in topicos)
                {
                    t.Botoes?.Clear();
                    await _repo.RemoverAsync(t);
                }
            }

            pagina.Botoes?.Clear();
            await _repo.RemoverAsync(pagina);
            await _repo.SalvarAlteracoesAsync();

            // Se removeu um tópico (cdPai != null), reordena os irmãos
            if (cdPai != null)
            {
                var topicos = (await _repo.ListarFilhosAsync(cdPai.Value))
                              .OrderBy(p => p.Ordem)
                              .ToList();

                for (int i = 0; i < topicos.Count; i++)
                    topicos[i].AtualizarOrdem(i + 1);

                await _repo.SalvarAlteracoesAsync();
            }
        }

        // Atualiza ordem entre duas páginas (identificadas por id)
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
