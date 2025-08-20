using Microsoft.AspNetCore.Mvc;
using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using System.Threading.Tasks;

namespace Paginas.Web.Controllers
{
    [Route("Botao")]
    public class BotaoController : Controller
    {
        private readonly IBotaoService _botaoService;

        public BotaoController(IBotaoService botaoService)
        {
            _botaoService = botaoService;
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
            return RedirectToAction("Detalhes", "Pagina", new { id = dto.CdPaginaIntrodutoria });
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
            return RedirectToAction("Detalhes", "Pagina", new { id = botao.CdPaginaIntrodutoria });
        }
    }
}
