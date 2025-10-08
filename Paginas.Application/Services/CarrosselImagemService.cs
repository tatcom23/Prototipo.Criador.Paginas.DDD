using Paginas.Application.DTOs;
using Paginas.Application.Mappers;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paginas.Application.Services
{
    public class CarrosselImagemService : ICarrosselImagemService
    {
        private readonly ICarrosselImagemRepository _repo;

        public CarrosselImagemService(ICarrosselImagemRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CarrosselImagemDTO>> ListarPorCarrosselAsync(int cdCarrossel)
        {
            var imagens = await _repo.ListarPorCarrosselAsync(cdCarrossel);
            return imagens.Select(i => i.ToDTO()).ToList();
        }

        public async Task<CarrosselImagemDTO> ObterPorIdAsync(int id)
        {
            var imagem = await _repo.ObterPorIdAsync(id);
            return imagem?.ToDTO();
        }

        public async Task CriarAsync(CarrosselImagemDTO model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var imagem = new CarrosselImagem(
                cdCarrossel: model.CdCarrossel,
                urlImagem: model.UrlImagem,
                ordem: model.Ordem,
                titulo: model.Titulo,
                descricao: model.Descricao
            );

            await _repo.AdicionarAsync(imagem);
            await _repo.SalvarAlteracoesAsync();
        }

        // NOVO: Criar imagem vinculada a um Carrossel específico
        public async Task<CarrosselImagemDTO> CriarAsync(CarrosselImagemDTO model, int cdCarrossel)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var imagem = new CarrosselImagem(
                cdCarrossel: cdCarrossel,
                urlImagem: model.UrlImagem,
                ordem: model.Ordem,
                titulo: model.Titulo,
                descricao: model.Descricao
            );

            await _repo.AdicionarAsync(imagem);
            await _repo.SalvarAlteracoesAsync();

            return imagem.ToDTO(); // Retorna DTO com Código preenchido
        }

        public async Task AtualizarAsync(int id, CarrosselImagemDTO model)
        {
            var imagem = await _repo.ObterPorIdAsync(id);
            if (imagem == null) return;

            imagem.Atualizar(
                urlImagem: model.UrlImagem,
                ordem: model.Ordem,
                titulo: model.Titulo,
                descricao: model.Descricao
            );

            await _repo.AtualizarAsync(imagem);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var imagem = await _repo.ObterPorIdAsync(id);
            if (imagem == null) return;

            await _repo.RemoverAsync(imagem);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task AtivarAsync(int id)
        {
            var imagem = await _repo.ObterPorIdAsync(id);
            if (imagem == null) return;

            imagem.Ativar();
            await _repo.AtualizarAsync(imagem);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task DesativarAsync(int id)
        {
            var imagem = await _repo.ObterPorIdAsync(id);
            if (imagem == null) return;

            imagem.Desativar();
            await _repo.AtualizarAsync(imagem);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task AtualizarOrdemAsync(int idA, int idB)
        {
            var a = await _repo.ObterPorIdAsync(idA);
            var b = await _repo.ObterPorIdAsync(idB);
            if (a == null || b == null) return;

            int ordemA = a.Ordem;
            int ordemB = b.Ordem;

            a.Atualizar(a.UrlImagem, ordemB, a.Titulo, a.Descricao);
            b.Atualizar(b.UrlImagem, ordemA, b.Titulo, b.Descricao);

            await _repo.AtualizarAsync(a);
            await _repo.AtualizarAsync(b);
            await _repo.SalvarAlteracoesAsync();
        }
    }
}
