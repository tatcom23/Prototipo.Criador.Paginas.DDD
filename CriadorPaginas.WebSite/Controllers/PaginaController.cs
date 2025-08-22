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
            _paginaService = paginaService ?? throw new ArgumentNullException(nameof(paginaService));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View(new PaginaDTO());
        }

        [HttpPost("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] PaginaDTO model, IFormFile BannerFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            // @todo: refatorar para selecionar o caminho da variável de ambiente appsettings
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

        // LISTAR com paginação (page = 1-based) e pageSize (limit)
        // Ex.: GET /Pagina/Listar?page=1&pageSize=10
        [HttpGet("Listar")]
        public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (itensDto, totalCount) = await _paginaService.ListarPaginadoAsync(page, pageSize);

            // garante listas não nulas
            if (itensDto != null)
            {
                foreach (var p in itensDto)
                {
                    p.PaginaFilhos ??= new List<PaginaDTO>();
                    p.Botoes ??= new List<BotaoDTO>();
                }
            }
            else
            {
                itensDto = new List<PaginaDTO>();
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;

            return View("Listar", itensDto); // View: Listar.cshtml (model List<PaginaDTO>)
        }

        [HttpGet("Detalhes/{id}")]
        public async Task<IActionResult> Detalhes(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            return View("Detalhes", pagina);
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

            return View("Editar", pagina);
        }

        [HttpPost("Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [FromForm] PaginaDTO model, IFormFile BannerFile)
        {
            if (!ModelState.IsValid)
                return View("Editar", model);

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
            return RedirectToAction("Listar", new { page = 1, pageSize = 10 });
        }

        [HttpGet("Excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            return View("Excluir", pagina);
        }

        [HttpPost("Excluir/{id}")]
        [ActionName("Excluir")]
        [ValidateAntiForgeryToken]
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

            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.PaginaFilhos.Clear();
            pagina.PaginaFilhos.AddRange(filhos);

            return View("Exibir", pagina);
        }

        // POST helper para atualizar ordem
        [HttpPost("AtualizarOrdem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarOrdem(int idA, int idB, int paginaId)
        {
            await _paginaService.AtualizarOrdemAsync(idA, idB);

            // Redireciona para a própria página usando o id passado
            return RedirectToAction("Gerenciar", new { id = paginaId });
        }

        // NOVA ACTION: tela completa (igual ao expandido atual) para gerenciar uma única página
        [HttpGet("Gerenciar/{id}")]
        public async Task<IActionResult> Gerenciar(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            // garante listas não nulas
            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.Botoes ??= new List<BotaoDTO>();

            // se desejar garantir que filhos venham do repo em ordem, pode descomentar:
            // var filhos = await _paginaService.ListarFilhosAsync(pagina.Codigo);
            // pagina.PaginaFilhos.Clear(); pagina.PaginaFilhos.AddRange(filhos);

            return View("Gerenciar", pagina); // Views/Pagina/Gerenciar.cshtml
        }
    }
}
