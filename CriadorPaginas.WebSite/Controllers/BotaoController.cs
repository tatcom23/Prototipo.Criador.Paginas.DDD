using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Paginas.Web.Controllers
{
    [Route("Botao")]
    public class BotaoController : Controller
    {
        private readonly IBotaoService _botaoService;
        private readonly IWebHostEnvironment _env;

        public BotaoController(IBotaoService botaoService, IWebHostEnvironment env)
        {
            _botaoService = botaoService;
            _env = env;
        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var botao = await _botaoService.BuscarPorIdAsync(id);
            if (botao == null)
                return NotFound();

            return View(botao);
        }

        [HttpPost("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, BotaoDTO botao)
        {
            if (!ModelState.IsValid)
                return View(botao);

            var existente = await _botaoService.BuscarPorIdAsync(id);
            if (existente == null)
                return NotFound();

            botao.Codigo = id; // garante que o DTO está correto para atualizar
            await _botaoService.AtualizarAsync(botao);

            TempData["Mensagem"] = "Botão atualizado com sucesso!";
            return RedirectToAction("Detalhes", "Pagina", new { id = botao.CdPaginaIntrodutoria });
        }


        [HttpGet("Excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var botao = await _botaoService.BuscarPorIdAsync(id);
            if (botao == null)
                return NotFound();

            return View(botao);
        }

        [HttpPost("Excluir/{id}")]
        [ActionName("Excluir")]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var botao = await _botaoService.BuscarPorIdAsync(id);
            if (botao == null)
                return NotFound();

            int paginaId = botao.CdPaginaIntrodutoria;

            await _botaoService.ExcluirAsync(id);

            TempData["Mensagem"] = "Botão excluído com sucesso!";
            return RedirectToAction("Detalhes", "Pagina", new { id = paginaId });
        }
    }
}
