using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting; // ✅ Para IWebHostEnvironment
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
        private readonly IWebHostEnvironment _env;

        public PaginaService(IPaginaRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        public async Task<List<Pagina>> ListarAsync()
        {
            return await _repo.ListarAsync();
        }

        public async Task<Pagina> BuscarPorIdAsync(int id)
        {
            return await _repo.BuscarPorIdAsync(id);
        }

        public async Task CriarAsync(PaginaDTO model)
        {
            var pagina = new Pagina
            {
                Titulo = model.Titulo,
                Conteudo = model.Conteudo,
                Url = model.Url,
                Banner = model.Banner,
                Tipo = 1,
                Criacao = DateTime.Now,
                Atualizacao = null,
                Status = true,
                Ordem = 1,
                Versao = 1,
                CdVersao = 1,
                Publicacao = false,
                Botoes = new List<Botao>()
            };

            string pastaArquivos = Path.Combine(_env.WebRootPath, "arquivos");
            Directory.CreateDirectory(pastaArquivos);

            // BOTÕES PRINCIPAIS
            if (model.Botoes != null)
            {
                foreach (var botaoVm in model.Botoes)
                {
                    string linkFinal = botaoVm.Link;

                    if (botaoVm.Arquivo != null && botaoVm.Arquivo.Length > 0)
                    {
                        string nomeArquivo = Guid.NewGuid() + Path.GetExtension(botaoVm.Arquivo.FileName);
                        string caminho = Path.Combine(pastaArquivos, nomeArquivo);
                        using var stream = new FileStream(caminho, FileMode.Create);
                        await botaoVm.Arquivo.CopyToAsync(stream);
                        linkFinal = "/arquivos/" + nomeArquivo;
                    }

                    if (!string.IsNullOrWhiteSpace(botaoVm.Nome) && !string.IsNullOrWhiteSpace(linkFinal))
                    {
                        pagina.Botoes.Add(new Botao
                        {
                            Nome = botaoVm.Nome,
                            Link = linkFinal,
                            Tipo = 1,
                            Linha = botaoVm.Linha,
                            Coluna = botaoVm.Coluna,
                            Criacao = DateTime.Now,
                            Status = true,
                            Versao = 1
                        });
                    }
                }
            }

            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAsync();

            // TÓPICOS
            if (model.Topicos != null && model.Topicos.Any())
            {
                int ordemAtual = 1;

                foreach (var topico in model.Topicos)
                {
                    var subpagina = new Pagina
                    {
                        Titulo = topico.Titulo,
                        Conteudo = topico.Conteudo,
                        Url = topico.Url,
                        Banner = topico.Banner,
                        Tipo = 2,
                        CdPai = pagina.Codigo,
                        Criacao = DateTime.Now,
                        Atualizacao = null,
                        Status = true,
                        Publicacao = false,
                        Ordem = ordemAtual++,
                        Versao = 1,
                        CdVersao = 1,
                        Botoes = new List<Botao>()
                    };

                    foreach (var b in topico.Botoes)
                    {
                        string linkFinal = b.Link;

                        if (b.Arquivo != null && b.Arquivo.Length > 0)
                        {
                            string nomeArquivo = Guid.NewGuid() + Path.GetExtension(b.Arquivo.FileName);
                            string caminho = Path.Combine(pastaArquivos, nomeArquivo);
                            using var stream = new FileStream(caminho, FileMode.Create);
                            await b.Arquivo.CopyToAsync(stream);
                            linkFinal = "/arquivos/" + nomeArquivo;
                        }

                        if (!string.IsNullOrWhiteSpace(b.Nome) && !string.IsNullOrWhiteSpace(linkFinal))
                        {
                            subpagina.Botoes.Add(new Botao
                            {
                                Nome = b.Nome,
                                Link = linkFinal,
                                Tipo = 1,
                                Linha = b.Linha,
                                Coluna = b.Coluna,
                                Criacao = DateTime.Now,
                                Status = true,
                                Versao = 1
                            });
                        }
                    }

                    await _repo.AdicionarAsync(subpagina);
                }

                await _repo.SalvarAsync();
            }
        }

        public async Task AtualizarAsync(int id, PaginaDTO model)
        {
            var pagina = await _repo.BuscarPorIdAsync(id);
            if (pagina == null) return;

            pagina.Titulo = model.Titulo;
            pagina.Conteudo = model.Conteudo;
            pagina.Url = model.Url;

            if (!string.IsNullOrWhiteSpace(model.Banner))
                pagina.Banner = model.Banner;

            pagina.Atualizacao = DateTime.Now;

            // === Adicionar novos botões ===
            if (model.Botoes != null && model.Botoes.Any())
            {
                string pastaArquivos = Path.Combine(_env.WebRootPath, "arquivos");
                Directory.CreateDirectory(pastaArquivos);

                foreach (var b in model.Botoes)
                {
                    string linkFinal = b.Link;

                    if (b.Arquivo != null && b.Arquivo.Length > 0)
                    {
                        string nomeArquivo = Guid.NewGuid() + Path.GetExtension(b.Arquivo.FileName);
                        string caminho = Path.Combine(pastaArquivos, nomeArquivo);
                        using var stream = new FileStream(caminho, FileMode.Create);
                        await b.Arquivo.CopyToAsync(stream);
                        linkFinal = "/arquivos/" + nomeArquivo;
                    }

                    if (!string.IsNullOrWhiteSpace(b.Nome) && !string.IsNullOrWhiteSpace(linkFinal))
                    {
                        pagina.Botoes.Add(new Botao
                        {
                            Nome = b.Nome,
                            Link = linkFinal,
                            Tipo = 1,
                            Linha = b.Linha,
                            Coluna = b.Coluna,
                            Criacao = DateTime.Now,
                            Status = true,
                            Versao = 1
                        });
                    }
                }
            }

            await _repo.AtualizarAsync(pagina);
            await _repo.SalvarAsync();
        }

        public async Task CriarComPaiAsync(PaginaDTO model, int cdPai)
        {
            var ordemAtualMax = (await _repo.ListarAsync())
                .Where(p => p.CdPai == cdPai)
                .Max(p => (int?)p.Ordem) ?? 0;

            var novaOrdem = ordemAtualMax + 1;

            var pagina = new Pagina
            {
                Titulo = model.Titulo,
                Conteudo = model.Conteudo,
                Url = model.Url,
                Banner = model.Banner,
                Tipo = 2,
                CdPai = cdPai,
                Criacao = DateTime.Now,
                Status = false,
                Ordem = novaOrdem,
                Versao = 1,
                CdVersao = 1,
                Botoes = new List<Botao>()
            };

            await _repo.AdicionarAsync(pagina);
            await _repo.SalvarAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var pagina = await _repo.BuscarPorIdAsync(id);
            if (pagina == null) return;

            var cdPai = pagina.CdPai;

            if (cdPai == null)
            {
                var topicos = (await _repo.ListarAsync()).Where(p => p.CdPai == pagina.Codigo).ToList();

                foreach (var topico in topicos)
                {
                    topico.Botoes?.Clear();
                    await _repo.ExcluirAsync(topico);
                }
            }

            pagina.Botoes?.Clear();
            await _repo.ExcluirAsync(pagina);
            await _repo.SalvarAsync();

            if (cdPai != null)
            {
                var topicos = (await _repo.ListarAsync()).Where(p => p.CdPai == cdPai).OrderBy(p => p.Ordem).ToList();

                for (int i = 0; i < topicos.Count; i++)
                {
                    topicos[i].Ordem = i + 1;
                    await _repo.AtualizarAsync(topicos[i]);
                }

                await _repo.SalvarAsync();
            }
        }

        public async Task AtualizarOrdemAsync(Pagina a, Pagina b)
        {
            await _repo.AtualizarAsync(a);
            await _repo.AtualizarAsync(b);
            await _repo.SalvarAsync();
        }
    }
}
