using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paginas.Application.DTOs;
using Paginas.Application.Services.Interfaces;
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

            if (BannerFile != null && BannerFile.Length > 0)
            {
                string nomeBanner = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeBanner);
                using var stream = new FileStream(caminho, FileMode.Create);
                await BannerFile.CopyToAsync(stream);
                model.Banner = "/imagens/" + nomeBanner;
            }

            await _paginaService.CriarAsync(model);

            TempData["Mensagem"] = "Página criada com sucesso!";
            return RedirectToAction("Listar");
        }

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var paginas = await _paginaService.ListarAsync();
            return View(paginas);
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

            var model = new PaginaDTO
            {
                Codigo = pagina.Codigo,
                Titulo = pagina.Titulo,
                Conteudo = pagina.Conteudo,
                Url = pagina.Url,
                Banner = pagina.Banner,
                CdPai = pagina.CdPai,
                Botoes = pagina.Botoes?.Select(b => new BotaoDTO
                {
                    Nome = b.Nome,
                    Link = b.Link,
                    Linha = b.Linha,
                    Coluna = b.Coluna
                }).ToList() ?? new List<BotaoDTO>()
            };

            return View(model);
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

            await _paginaService.AtualizarAsync(id, model);

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
            var pagina = (await _paginaService.ListarAsync())
                .FirstOrDefault(p => p.Url == url && p.Tipo == 1);

            if (pagina == null)
                return NotFound();

            pagina.PaginaFilhos = (await _paginaService.ListarAsync())
                .Where(p => p.CdPai == pagina.Codigo)
                .ToList();

            return View("Exibir", pagina);
        }
    }
}
