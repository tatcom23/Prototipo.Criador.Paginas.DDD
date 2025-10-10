using Paginas.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginas.Application.Services.Interfaces
{
    public interface IPaginaService
    {
        // Para consumo pela camada MVC/Views
        Task<List<PaginaDTO>> ListarAsync();
        Task<PaginaDTO> BuscarPorIdAsync(int id);
        Task<List<PaginaDTO>> ListarFilhosAsync(int cdPai);

        // Paginação: retorna items e total count (somente raiz por padrão)
        Task<(List<PaginaDTO> Items, int TotalCount)> ListarPaginadoAsync(int page, int pageSize);

        // Operações de escrita (entrada em DTO)
        Task CriarAsync(PaginaDTO model, string webRootPath);
        Task AtualizarAsync(int id, PaginaDTO model, string webRootPath);
        Task ExcluirAsync(int id);

        // Criar um filho passando o id do pai (opcional)
        Task CriarComPaiAsync(PaginaDTO model, int cdPai);

        // Atualizar ordem entre duas páginas por id (não expõe entidades)
        Task AtualizarOrdemAsync(int idA, int idB);
        Task<DashboardViewModel> ObterDadosDashboardAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

    }
}
