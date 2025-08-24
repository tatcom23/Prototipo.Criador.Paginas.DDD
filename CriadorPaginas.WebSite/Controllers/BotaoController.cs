using Microsoft.AspNetCore.Mvc;
using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paginas.Web.Controllers
{
    [Route("Botao")]
    public class BotaoController : Controller
    {
        private readonly IBotaoService _botaoService;
        private readonly IPaginaRepository _paginaRepository; // Adicionado

        public BotaoController(IBotaoService botaoService, IPaginaRepository paginaRepository)
        {
            _botaoService = botaoService;
            _paginaRepository = paginaRepository;
        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var botao = await _botaoService.BuscarPorIdAsync(id);
            if (botao == null) return NotFound();

            return View(botao);
        }

        [HttpPost("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromForm] BotaoDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            dto.Codigo = id;
            await _botaoService.AtualizarAsync(dto);

            TempData["Mensagem"] = "Botão atualizado com sucesso!";

            // ✅ Obter o id da página principal (raiz)
            var idPaginaPrincipal = await ObterIdPaginaPrincipal(dto.CdPaginaIntrodutoria);
            return RedirectToAction("Gerenciar", "Pagina", new { id = idPaginaPrincipal });
        }

        [HttpGet("Excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var botao = await _botaoService.BuscarPorIdAsync(id);
            if (botao == null) return NotFound();

            return View(botao);
        }

        [HttpPost("Excluir/{id}")]
        [ActionName("Excluir")]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var botao = await _botaoService.BuscarPorIdAsync(id);
            if (botao == null) return NotFound();

            await _botaoService.ExcluirAsync(id);

            TempData["Mensagem"] = "Botão excluído com sucesso!";

            // ✅ Obter o id da página principal (raiz)
            var idPaginaPrincipal = await ObterIdPaginaPrincipal(botao.CdPaginaIntrodutoria);
            return RedirectToAction("Gerenciar", "Pagina", new { id = idPaginaPrincipal });
        }

        // POST helper para atualizar ordem
        [HttpPost("AtualizarOrdem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarOrdem(int idA, int idB, int paginaId)
        {
            await _botaoService.AtualizarOrdemAsync(idA, idB);

            var idPaginaPrincipal = await ObterIdPaginaPrincipal(paginaId);
            return RedirectToAction("Gerenciar", "Pagina", new { id = idPaginaPrincipal });
        }

        // DTO para receber a requisição
        public class AtualizarOrdemLoteRequest
        {
            public List<int> idsEmOrdem { get; set; }
            public string tipo { get; set; } // "principal" ou "topico"
        }

        [HttpPost("AtualizarOrdemEmLote")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarOrdemEmLote([FromBody] AtualizarOrdemLoteRequest request)
        {
            if (request?.idsEmOrdem == null || !request.idsEmOrdem.Any())
                return BadRequest("Lista de IDs inválida.");

            for (int i = 0; i < request.idsEmOrdem.Count; i++)
            {
                var botaoId = request.idsEmOrdem[i];
                await _botaoService.AtualizarOrdemIndividualAsync(botaoId, i + 1);
            }

            return Ok(new { success = true });
        }

        // ✅ Método auxiliar: sobe na árvore até a raiz
        private async Task<int> ObterIdPaginaPrincipal(int paginaId)
        {
            var pagina = await _paginaRepository.ObterPorIdAsync(paginaId);
            if (pagina == null) return paginaId;

            // Sobe até a raiz (CdPai nulo)
            while (pagina.CdPai.HasValue)
            {
                pagina = await _paginaRepository.ObterPorIdAsync(pagina.CdPai.Value);
                if (pagina == null) break;
            }

            return pagina?.Codigo ?? paginaId;
        }
    }
}