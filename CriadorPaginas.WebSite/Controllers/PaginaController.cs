using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
using Paginas.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paginas.Web.Controllers
{
    [Route("Pagina")]
    public class PaginaController : Controller
    {
        private readonly IPaginaService _paginaService;
        private readonly IWebHostEnvironment _env;

        public PaginaController(IPaginaService paginaService, IWebHostEnvironment env)
        {
            _paginaService = paginaService;
            _env = env;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View(new PaginaDTO());
        }

        [HttpPost("Index")]
        public async Task<IActionResult> Index([FromForm] PaginaDTO model, IFormFile BannerFile)
        {
            if (!ModelState.IsValid)
                return View(model);
            //@todo: refatorar para selecionar o caminho da variável de ambiente appsettings
            if (BannerFile != null && BannerFile.Length > 0)
            {
                string nomeBanner = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeBanner);
                using var stream = new FileStream(caminho, FileMode.Create);
                await BannerFile.CopyToAsync(stream);
                model.Banner = "/imagens/" + nomeBanner;
            }

            await _paginaService.CriarAsync(model, _env.WebRootPath);

            TempData["Mensagem"] = "Página criada com sucesso!";
            return RedirectToAction("Listar");
        }

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var paginas = await _paginaService.ListarAsync(); // List<PaginaDTO>

            // Organiza hierarquia de páginas principais e filhos
            var paginasPrincipais = paginas
                .Where(p => p.CdPai == null)
                .OrderBy(p => p.Ordem)
                .ToList();

            foreach (var pagina in paginasPrincipais)
            {
                var filhos = paginas
                    .Where(f => f.CdPai == pagina.Codigo)
                    .OrderBy(f => f.Ordem)
                    .ToList();

                // segura mesmo se PaginaFilhos for só-get com lista inicializada
                pagina.PaginaFilhos.Clear();
                pagina.PaginaFilhos.AddRange(filhos);
            }

            return View(paginasPrincipais); // View recebe List<PaginaDTO>
        }

        [HttpGet("Detalhes/{id}")]
        public async Task<IActionResult> Detalhes(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            return View(pagina);
        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            // garante listas não nulas para a View
            pagina.Botoes ??= new List<BotaoDTO>();
            pagina.PaginaFilhos ??= new List<PaginaDTO>();

            return View(pagina);
        }

        [HttpPost("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromForm] PaginaDTO model, IFormFile BannerFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var paginaExistente = await _paginaService.BuscarPorIdAsync(id);
            if (paginaExistente == null)
                return NotFound();

            if (BannerFile != null && BannerFile.Length > 0)
            {
                string nomeBanner = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeBanner);
                using var stream = new FileStream(caminho, FileMode.Create);
                await BannerFile.CopyToAsync(stream);
                model.Banner = "/imagens/" + nomeBanner;
            }
            else
            {
                model.Banner = paginaExistente.Banner;
            }

            await _paginaService.AtualizarAsync(id, model, _env.WebRootPath);

            TempData["Mensagem"] = "Página atualizada com sucesso!";
            return RedirectToAction("Listar");
        }

        [HttpGet("Excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            return View(pagina);
        }

        [HttpPost("Excluir/{id}")]
        [ActionName("Excluir")]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            await _paginaService.ExcluirAsync(id);

            TempData["Mensagem"] = "Página excluída com sucesso!";
            return RedirectToAction("Listar");
        }

        [HttpGet("exibir/{url}", Name = "Pagina_Exibir")]
        public async Task<IActionResult> Exibir(string url)
        {
            var paginas = await _paginaService.ListarAsync();
            var pagina = paginas
                .FirstOrDefault(p => p.Url == url && p.Tipo == (int)TipoPagina.Principal);

            if (pagina == null)
                return NotFound();

            var filhos = paginas
                .Where(p => p.CdPai == pagina.Codigo)
                .OrderBy(p => p.Ordem)
                .ToList();

            pagina.PaginaFilhos.Clear();
            pagina.PaginaFilhos.AddRange(filhos);

            return View("Exibir", pagina);
        }

        // POST helper para atualizar ordem (chamada pela View Listar via form)
        [HttpPost("AtualizarOrdem")]
        public async Task<IActionResult> AtualizarOrdem(int idA, int idB)
        {
            await _paginaService.AtualizarOrdemAsync(idA, idB);
            return RedirectToAction("Listar");
        }
    }
}
