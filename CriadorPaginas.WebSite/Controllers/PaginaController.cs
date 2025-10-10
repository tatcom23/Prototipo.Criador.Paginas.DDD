using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paginas.Application.DTOs;
using Paginas.Application.Services;
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
        private readonly ICarrosselService _carrosselService;
        private readonly ICarrosselImagemService _carrosselImagemService;
        private readonly IWebHostEnvironment _env;
        private readonly IDashboardService _dashboardService;

        public PaginaController(
            IPaginaService paginaService,
            ICarrosselService carrosselService,
            ICarrosselImagemService carrosselImagemService,
            IWebHostEnvironment env,
            IDashboardService dashboardService) // ✅ Adicionei aqui
        {
            _paginaService = paginaService ?? throw new ArgumentNullException(nameof(paginaService));
            _carrosselService = carrosselService ?? throw new ArgumentNullException(nameof(carrosselService));
            _carrosselImagemService = carrosselImagemService ?? throw new ArgumentNullException(nameof(carrosselImagemService));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        }


        [HttpGet("Index")]
        public IActionResult Index() => View(new PaginaDTO());

        [HttpPost("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            [FromForm] PaginaDTO model,
            IFormFile BannerFile,
            [FromForm] List<IFormFile> CarrosselFiles,
            [FromForm] string carrosselTitulo)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Validação URL página principal
            if (model.CdPai == null)
            {
                var paginas = await _paginaService.ListarAsync();
                if (paginas.Any(p => p.Url == model.Url && p.Codigo != model.Codigo))
                {
                    ModelState.AddModelError("Url", "Já existe uma página principal com esta URL.");
                    return View(model);
                }
            }

            // Upload Banner
            if (BannerFile != null && BannerFile.Length > 0)
            {
                string nomeBanner = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeBanner);
                using var stream = new FileStream(caminho, FileMode.Create);
                await BannerFile.CopyToAsync(stream);
                model.Banner = "/imagens/" + nomeBanner;
            }

            // Gerar slug para tópicos sem URL
            if (model.PaginaFilhos != null)
            {
                foreach (var topico in model.PaginaFilhos)
                {
                    if (string.IsNullOrWhiteSpace(topico.Url))
                        topico.Url = GerarSlug(topico.Titulo);
                }
            }

            // Salvar Página principal
            await _paginaService.CriarAsync(model, _env.WebRootPath);

            // Upload Carrossel e imagens
            if (CarrosselFiles != null && CarrosselFiles.Any())
            {
                // Cria novo carrossel
                var carrosselDto = new CarrosselDTO
                {
                    Titulo = string.IsNullOrWhiteSpace(carrosselTitulo) ? "Carrossel principal" : carrosselTitulo,
                    Imagens = new List<CarrosselImagemDTO>()
                };

                // Salvar carrossel no banco
                var carrosselEntity = await _carrosselService.CriarAsync(carrosselDto, model.Codigo);

                int ordem = 1;
                foreach (var file in CarrosselFiles)
                {
                    if (file.Length <= 0) continue;

                    string nomeImg = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeImg);
                    using var stream = new FileStream(caminho, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var imagemDto = new CarrosselImagemDTO
                    {
                        Titulo = file.FileName,
                        UrlImagem = "/imagens/" + nomeImg,
                        Ordem = ordem++
                    };

                    // Salvar cada imagem no banco
                    await _carrosselImagemService.CriarAsync(imagemDto, carrosselEntity.Codigo);
                }
            }

            TempData["Mensagem"] = "Página criada com sucesso!";
            return model.CdPai == null
                ? RedirectToAction("Listar")
                : RedirectToAction("Gerenciar", new { id = model.CdPai });
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
            if (pagina == null) return NotFound();

            pagina.Botoes ??= new List<BotaoDTO>();
            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.Carrosseis ??= new List<CarrosselDTO>();

            return View("Editar", pagina);
        }

        [HttpPost("Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            [FromForm] PaginaDTO model,
            IFormFile BannerFile,
            [FromForm] List<IFormFile> CarrosselFiles,
            [FromForm] string carrosselTitulo)
        {
            if (!ModelState.IsValid)
                return View("Editar", model);

            var paginaExistente = await _paginaService.BuscarPorIdAsync(id);
            if (paginaExistente == null) return NotFound();

            bool ehTopico = paginaExistente.CdPai.HasValue;

            // Validação URL página principal
            if (!ehTopico && model.Url != paginaExistente.Url)
            {
                var paginas = await _paginaService.ListarAsync();
                if (paginas.Any(p => p.Url == model.Url && p.Codigo != id))
                {
                    ModelState.AddModelError("Url", "Já existe uma página principal com esta URL.");
                    return View("Editar", model);
                }
            }

            // Upload Banner
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

            // Gerar slug para tópicos
            if (model.PaginaFilhos != null)
            {
                foreach (var topico in model.PaginaFilhos)
                {
                    if (string.IsNullOrWhiteSpace(topico.Url))
                        topico.Url = GerarSlug(topico.Titulo);
                }
            }

            // Atualizar página
            await _paginaService.AtualizarAsync(id, model, _env.WebRootPath);

            // Upload Carrossel adicional
            if (CarrosselFiles != null && CarrosselFiles.Any())
            {
                var carrosselDto = new CarrosselDTO
                {
                    Titulo = string.IsNullOrWhiteSpace(carrosselTitulo) ? "Carrossel adicional" : carrosselTitulo,
                    Imagens = new List<CarrosselImagemDTO>()
                };

                var carrosselEntity = await _carrosselService.CriarAsync(carrosselDto, id);

                int ordem = 1;
                foreach (var file in CarrosselFiles)
                {
                    if (file.Length <= 0) continue;

                    string nomeImg = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeImg);
                    using var stream = new FileStream(caminho, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var imagemDto = new CarrosselImagemDTO
                    {
                        Titulo = file.FileName,
                        UrlImagem = "/imagens/" + nomeImg,
                        Ordem = ordem++
                    };

                    await _carrosselImagemService.CriarAsync(imagemDto, carrosselEntity.Codigo);
                }
            }

            TempData["Mensagem"] = "Página atualizada com sucesso!";
            return ehTopico
                ? RedirectToAction("Gerenciar", new { id = paginaExistente.CdPai.Value })
                : RedirectToAction("Gerenciar", new { id = id });
        }

        [HttpGet("Excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null) return NotFound();
            return View("Excluir", pagina);
        }

        [HttpPost("Excluir/{id}")]
        [ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null) return NotFound();

            bool ehTopico = pagina.CdPai.HasValue;
            int? cdPai = pagina.CdPai;

            await _paginaService.ExcluirAsync(id);

            TempData["Mensagem"] = "Página excluída com sucesso!";

            return ehTopico
                ? RedirectToAction("Gerenciar", new { id = cdPai.Value })
                : RedirectToAction("Listar");
        }

        [HttpGet("exibir/{url}", Name = "Pagina_Exibir")]
        public async Task<IActionResult> Exibir(string url)
        {
            var paginas = await _paginaService.ListarAsync();
            var pagina = paginas.FirstOrDefault(p => p.Url == url && p.Tipo == (int)TipoPagina.Principal);

            if (pagina == null) return NotFound();

            var filhos = paginas.Where(p => p.CdPai == pagina.Codigo).OrderBy(p => p.Ordem).ToList();
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
            if (pagina == null) return NotFound();

            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.Botoes ??= new List<BotaoDTO>();
            pagina.Carrosseis ??= new List<CarrosselDTO>();

            return View("Gerenciar", pagina);
        }

        private string GerarSlug(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo)) return Guid.NewGuid().ToString();

            return titulo
                .ToLowerInvariant()
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => char.IsLetterOrDigit(c) || c == ' ')
                .Aggregate("", (s, c) => s += c)
                .Replace(" ", "-")
                .Trim('-');
        }

        public async Task<IActionResult> Dashboard(string periodo, DateTime? dataInicio, DateTime? dataFim)
        {
            DateTime inicio = DateTime.MinValue;
            DateTime fim = DateTime.Now;

            switch (periodo)
            {
                case "ultimoMes":
                    inicio = DateTime.Now.AddMonths(-1);
                    break;
                case "ultimoTrimestre":
                    inicio = DateTime.Now.AddMonths(-3);
                    break;
                case "ultimoSemestre":
                    inicio = DateTime.Now.AddMonths(-6);
                    break;
                case "ultimoAno":
                    inicio = DateTime.Now.AddYears(-1);
                    break;
                case "personalizado":
                    if (dataInicio.HasValue && dataFim.HasValue)
                    {
                        inicio = dataInicio.Value;
                        fim = dataFim.Value;
                    }
                    break;
                case "todos":
                default:
                    inicio = DateTime.MinValue;
                    fim = DateTime.Now;
                    break;
            }

            var dados = await _paginaService.ObterDadosDashboardAsync(inicio, fim);
            return View(dados);
        }

        [HttpPost]
        public async Task<IActionResult> DashboardPdf(string graficoBase64)
        {
            byte[] graficoBytes = null;

            if (!string.IsNullOrEmpty(graficoBase64))
            {
                graficoBase64 = graficoBase64.Replace("data:image/png;base64,", "");
                graficoBytes = Convert.FromBase64String(graficoBase64);
            }

            // Recupera os dados reais do dashboard
            var dados = await _paginaService.ObterDadosDashboardAsync();

            var pdf = _dashboardService.GerarPdf(dados, graficoBytes);
            return File(pdf, "application/pdf", "Dashboard.pdf");
        }

    }
}
