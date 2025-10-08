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

        public CarrosselService(ICarrosselRepository repo)
        {
            _repo = repo;
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

            // Cria a entidade usando o construtor que recebe o cdPagina
            var carrossel = new Carrossel(
                cdPagina: paginaId,
                titulo: model.Titulo,
                descricao: model.Descricao
            );

            // Adiciona no repositório
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

            carrossel.Imagens.Clear();
            await _repo.RemoverAsync(carrossel);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task AdicionarImagemAsync(int cdCarrossel, CarrosselImagemDTO imagem)
        {
            var carrossel = await _repo.ObterPorIdAsync(cdCarrossel);
            if (carrossel == null) throw new ArgumentNullException(nameof(carrossel));

            carrossel.AdicionarImagem(imagem.ToEntity());
            await _repo.AtualizarAsync(carrossel);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task AtualizarImagemAsync(int id, CarrosselImagemDTO imagem)
        {
            var img = await _repo.ObterImagemPorIdAsync(id);
            if (img == null) return;

            img.Atualizar(imagem.UrlImagem, imagem.Ordem, imagem.Titulo, imagem.Descricao);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task ExcluirImagemAsync(int id)
        {
            var img = await _repo.ObterImagemPorIdAsync(id);
            if (img == null) return;

            var carrossel = img.Carrossel;
            carrossel.RemoverImagem(img);
            await _repo.SalvarAlteracoesAsync();
        }

        public async Task AtualizarOrdemImagensAsync(int cdCarrossel, List<int> ordemIds)
        {
            var carrossel = await _repo.ObterPorIdAsync(cdCarrossel);
            if (carrossel == null) return;

            for (int i = 0; i < ordemIds.Count; i++)
            {
                var img = carrossel.Imagens.FirstOrDefault(x => x.Codigo == ordemIds[i]);
                if (img != null)
                    img.Atualizar(img.UrlImagem, i + 1, img.Titulo, img.Descricao);
            }

            await _repo.SalvarAlteracoesAsync();
        }
    }
}
