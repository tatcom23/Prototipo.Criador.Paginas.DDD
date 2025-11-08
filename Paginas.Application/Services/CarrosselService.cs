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
    public class CarrosselService : ICarrosselService
    {
        private readonly ICarrosselRepository _repo;
        private readonly ICarrosselImagemRepository _imagemRepo;

        public CarrosselService(
            ICarrosselRepository repo,
            ICarrosselImagemRepository imagemRepo)
        {
            _repo = repo;
            _imagemRepo = imagemRepo;
        }

        public async Task<List<CarrosselDTO>> ListarPorPaginaAsync(int cdPagina)
        {
            var carrosseis = await _repo.ListarPorPaginaAsync(cdPagina);
            return carrosseis.Select(c => c.ToDTO()).ToList();
        }

        public async Task<CarrosselDTO> BuscarPorIdAsync(int id)
        {
            var carrossel = await _repo.ObterPorIdAsync(id);
            return carrossel?.ToDTO();
        }

        public async Task<CarrosselDTO> CriarAsync(CarrosselDTO model, int paginaId)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var carrossel = new Carrossel(
                cdPagina: paginaId,
                titulo: model.Titulo,
                descricao: model.Descricao
            );

            await _repo.AdicionarAsync(carrossel);
            await _repo.SalvarAlteracoesAsync();

            return carrossel.ToDTO();
        }

        public async Task AtualizarAsync(int id, CarrosselDTO model)
        {
            var carrossel = await _repo.ObterPorIdAsync(id);
            if (carrossel == null) return;

            carrossel.Atualizar(model.Titulo, model.Descricao);
            await _repo.AtualizarAsync(carrossel);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var carrossel = await _repo.ObterPorIdAsync(id);
            if (carrossel == null) return;

            // remove imagens
            _repo.RemoverAsync(carrossel);
            await _repo.SalvarAlteracoesAsync();
        }

        // SALVAR IMAGEM (corrigido pra usar imagemRepo)
        public async Task AdicionarImagemAsync(int cdCarrossel, CarrosselImagemDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var imagem = dto.ToEntity();
            imagem.CdCarrossel = cdCarrossel;

            await _imagemRepo.AdicionarAsync(imagem);
            await _imagemRepo.SalvarAlteracoesAsync();
        }

        // usa repo correto
        public async Task AtualizarImagemAsync(int id, CarrosselImagemDTO dto)
        {
            var img = await _imagemRepo.ObterPorIdAsync(id);
            if (img == null) return;

            img.Atualizar(dto.UrlImagem, dto.Ordem, dto.Titulo, dto.Descricao);

            await _imagemRepo.AtualizarAsync(img);
            await _imagemRepo.SalvarAlteracoesAsync();
        }

        public async Task ExcluirImagemAsync(int id)
        {
            var img = await _imagemRepo.ObterPorIdAsync(id);
            if (img == null) return;

            await _imagemRepo.RemoverAsync(img);
            await _imagemRepo.SalvarAlteracoesAsync();
        }

        public async Task AtualizarOrdemImagensAsync(int cdCarrossel, List<int> ordemIds)
        {
            var imagens = await _imagemRepo.ListarPorCarrosselAsync(cdCarrossel);

            for (int i = 0; i < ordemIds.Count; i++)
            {
                var img = imagens.FirstOrDefault(x => x.Codigo == ordemIds[i]);
                if (img != null)
                {
                    img.Atualizar(img.UrlImagem, i + 1, img.Titulo, img.Descricao);
                    await _imagemRepo.AtualizarAsync(img);
                }
            }

            await _imagemRepo.SalvarAlteracoesAsync();
        }
    }
}
