using Redirect.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redirect.Application.Services.Interfaces
{
    /// <summary>
    /// Serviço responsável por gerenciar origens e seus destinos de redirecionamento.
    /// Inclui toda a lógica de CRUD e validação de períodos.
    /// </summary>
    public interface IRedirecionamentoOrigemService
    {
        /// <summary>
        /// Obtém uma origem de redirecionamento completa (com destinos) pela URL de origem.
        /// </summary>
        Task<RedirecionamentoOrigemDTO?> ObterPorUrlOrigemAsync(string urlOrigem);

        /// <summary>
        /// Obtém uma origem de redirecionamento pelo identificador.
        /// </summary>
        Task<RedirecionamentoOrigemDTO?> ObterPorIdAsync(int id);

        /// <summary>
        /// Lista todas as origens cadastradas com seus destinos.
        /// </summary>
        Task<IEnumerable<RedirecionamentoOrigemDTO>> ObterTodosAsync();

        /// <summary>
        /// Adiciona uma nova origem com seus destinos associados.
        /// Inclui validação automática das datas.
        /// </summary>
        Task AdicionarAsync(RedirecionamentoOrigemDTO dto);

        /// <summary>
        /// Atualiza uma origem e seus destinos.
        /// Aplica as mesmas regras de validação de períodos.
        /// </summary>
        Task AtualizarAsync(RedirecionamentoOrigemDTO dto);

        /// <summary>
        /// Remove uma origem e todos os destinos vinculados a ela.
        /// </summary>
        Task RemoverAsync(int id);

        /// <summary>
        /// Seleciona o destino válido com base nas datas (usado pelo middleware).
        /// </summary>
        RedirecionamentoDestinoDTO? SelecionarDestinoValido(RedirecionamentoOrigemDTO origem);
    }
}

