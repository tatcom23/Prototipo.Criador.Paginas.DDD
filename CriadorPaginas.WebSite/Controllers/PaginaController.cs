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

            // ✅ 2. Validar duplicidade de URL (somente para páginas principais)
            if (model.CdPai == null) // é página principal
            {
                var paginas = await _paginaService.ListarAsync();
                if (paginas.Any(p => p.Url == model.Url && p.Codigo != model.Codigo))
                {
                    ModelState.AddModelError("Url", "Já existe uma página principal com esta URL.");
                    return View(model);
                }
            }

            if (BannerFile != null && BannerFile.Length > 0)
            {
                string nomeBanner = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeBanner);
                using var stream = new FileStream(caminho, FileMode.Create);
                await BannerFile.CopyToAsync(stream);
                model.Banner = "/imagens/" + nomeBanner;
            }

            // ✅ 3. Gerar âncoras para tópicos com base no título
            if (model.PaginaFilhos != null)
            {
                foreach (var topico in model.PaginaFilhos)
                {
                    if (string.IsNullOrWhiteSpace(topico.Url))
                    {
                        topico.Url = GerarSlug(topico.Titulo);
                    }
                }
            }

            await _paginaService.CriarAsync(model, _env.WebRootPath);

            TempData["Mensagem"] = "Página criada com sucesso!";

            // ✅ 1. Após criar página principal → redireciona para Listar
            if (model.CdPai == null)
            {
                return RedirectToAction("Listar");
            }

            // Se for tópico, vai para Gerenciar da página pai
            return RedirectToAction("Gerenciar", new { id = model.CdPai });
        }

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (itensDto, totalCount) = await _paginaService.ListarPaginadoAsync(page, pageSize);

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

            return View("Listar", itensDto);
        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

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

            bool ehTopico = paginaExistente.CdPai.HasValue;

            // ✅ Validar URL duplicada (somente para páginas principais)
            if (!ehTopico && model.Url != paginaExistente.Url)
            {
                var paginas = await _paginaService.ListarAsync();
                if (paginas.Any(p => p.Url == model.Url && p.Codigo != id))
                {
                    ModelState.AddModelError("Url", "Já existe uma página principal com esta URL.");
                    return View("Editar", model);
                }
            }

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

            // ✅ Gerar slug para tópicos sem URL
            if (model.PaginaFilhos != null)
            {
                foreach (var topico in model.PaginaFilhos)
                {
                    if (string.IsNullOrWhiteSpace(topico.Url))
                    {
                        topico.Url = GerarSlug(topico.Titulo);
                    }
                }
            }

            await _paginaService.AtualizarAsync(id, model, _env.WebRootPath);

            TempData["Mensagem"] = "Página atualizada com sucesso!";

            if (ehTopico)
            {
                return RedirectToAction("Gerenciar", "Pagina", new { id = paginaExistente.CdPai.Value });
            }

            return RedirectToAction("Gerenciar", "Pagina", new { id = model.Codigo });
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
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            bool ehTopico = pagina.CdPai.HasValue;
            int? cdPai = pagina.CdPai;

            await _paginaService.ExcluirAsync(id);

            TempData["Mensagem"] = "Página excluída com sucesso!";

            if (ehTopico)
            {
                return RedirectToAction("Gerenciar", "Pagina", new { id = cdPai.Value });
            }

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

        [HttpPost("AtualizarOrdem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarOrdem(int idA, int idB, int paginaId)
        {
            await _paginaService.AtualizarOrdemAsync(idA, idB);
            return RedirectToAction("Gerenciar", new { id = paginaId });
        }

        [HttpGet("Gerenciar/{id}")]
        public async Task<IActionResult> Gerenciar(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.Botoes ??= new List<BotaoDTO>();

            return View("Gerenciar", pagina);
        }

        // ✅ Método para gerar slug a partir do título
        private string GerarSlug(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return Guid.NewGuid().ToString();

            return titulo
                .ToLowerInvariant()
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => char.IsLetterOrDigit(c) || c == ' ')
                .Aggregate("", (s, c) => s += c)
                .Replace(" ", "-")
                .Trim('-');
        }
    }
}