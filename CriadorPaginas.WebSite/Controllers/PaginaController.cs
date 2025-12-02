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
            IDashboardService dashboardService)
        {
            _paginaService = paginaService;
            _carrosselService = carrosselService;
            _carrosselImagemService = carrosselImagemService;
            _env = env;
            _dashboardService = dashboardService;
        }

        // =======================================================================
        //  LISTAGEM
        // =======================================================================

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar(int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var (itens, total) = await _paginaService.ListarPaginadoAsync(page, pageSize);

            itens ??= new List<PaginaDTO>();

            foreach (var p in itens)
            {
                p.PaginaFilhos ??= new List<PaginaDTO>();
                p.Botoes ??= new List<BotaoDTO>();
            }

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = total;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            return View("Listar", itens);
        }

        // =======================================================================
        //  CRIAÇÃO 
        // =======================================================================

        [HttpGet("Index")]
        public IActionResult Index()
        {
            var modelo = new PaginaDTO
            {
                PaginaFilhos = new List<PaginaDTO>(),
                Carrosseis = new List<CarrosselDTO>(),
                Botoes = new List<BotaoDTO>()
            };

            return View("Index", modelo);
        }


        [HttpPost("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            PaginaDTO model,
            IFormFile BannerFile,
            List<IFormFile> CarrosselFiles,
            string carrosselTitulo,
            // NOVO CAMPO: Descrição do Carrossel
            string carrosselDescricao,
            // NOVOS CAMPOS: Listas de Títulos e Descrições das Imagens
            List<string> CarrosselImagensTitulos,
            List<string> CarrosselImagensDescricoes)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Garantir URL única somente para páginas principais
            if (model.CdPai == null)
            {
                var paginas = await _paginaService.ListarAsync();
                if (paginas.Any(p => string.Equals(p.Url, model.Url, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Url", "Já existe uma página principal com esta URL.");
                    return View(model);
                }
            }

            // Upload do Banner
            if (BannerFile != null && BannerFile.Length > 0)
            {
                string nome = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nome);

                using var stream = new FileStream(caminho, FileMode.Create);
                await BannerFile.CopyToAsync(stream);

                model.Banner = "/imagens/" + nome;
            }

            // Gerar URLs faltantes dos filhos
            if (model.PaginaFilhos != null)
            {
                foreach (var f in model.PaginaFilhos)
                    if (string.IsNullOrWhiteSpace(f.Url))
                        f.Url = GerarSlug(f.Titulo);
            }

            // Criar página (método de serviço que retorna void)
            await _paginaService.CriarAsync(model, _env.WebRootPath);

            // Recuperar a página criada consultando todas e procurando pela URL (case-insensitive)
            var todasPaginas = await _paginaService.ListarAsync();
            var paginaCriada = todasPaginas
                .FirstOrDefault(p => string.Equals(p.Url, model.Url, StringComparison.OrdinalIgnoreCase)
                                     && p.CdPai == model.CdPai);

            if (paginaCriada == null)
            {
                TempData["Mensagem"] = "Página criada, mas ocorreu um problema ao recuperar seu identificador. Verifique o sistema.";
                return model.CdPai == null
                    ? RedirectToAction("Listar")
                    : RedirectToAction("Gerenciar", new { id = model.CdPai });
            }

            // Criar carrossel (se houver arquivos)
            if (CarrosselFiles != null && CarrosselFiles.Any())
            {
                var carrossel = await _carrosselService.CriarAsync(
                    new CarrosselDTO
                    {
                        // Inclui a descrição do carrossel
                        Titulo = string.IsNullOrWhiteSpace(carrosselTitulo)
                            ? "Carrossel principal"
                            : carrosselTitulo,
                        Descricao = carrosselDescricao // <--- AQUI!
                    },
                    paginaCriada.Codigo);

                int ordem = 1;

                // Itera sobre os arquivos e usa o índice para pegar o título/descrição correspondente
                for (int i = 0; i < CarrosselFiles.Count; i++)
                {
                    var file = CarrosselFiles[i];

                    if (file.Length == 0) continue;

                    // Tenta obter o Título e a Descrição do CarrosselImagensTitulos/Descricoes
                    string tituloImagem = (CarrosselImagensTitulos != null && i < CarrosselImagensTitulos.Count)
                        ? CarrosselImagensTitulos[i]
                        : file.FileName;

                    string descricaoImagem = (CarrosselImagensDescricoes != null && i < CarrosselImagensDescricoes.Count)
                        ? CarrosselImagensDescricoes[i]
                        : string.Empty;

                    string nomeImg = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeImg);

                    using var stream = new FileStream(caminho, FileMode.Create);
                    await file.CopyToAsync(stream);

                    await _carrosselImagemService.CriarAsync(
                        new CarrosselImagemDTO
                        {
                            Titulo = tituloImagem, // <--- AQUI!
                            Descricao = descricaoImagem, // <--- AQUI!
                            UrlImagem = "/imagens/" + nomeImg,
                            Ordem = ordem++
                        },
                        carrossel.Codigo);
                }
            }

            TempData["Mensagem"] = "Página criada com sucesso!";

            return model.CdPai == null
                ? RedirectToAction("Listar")
                : RedirectToAction("Gerenciar", new { id = model.CdPai });
        }

        // =======================================================================
        //  EDIÇÃO
        // =======================================================================

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);

            if (pagina == null)
                return NotFound();

            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.Carrosseis ??= new List<CarrosselDTO>();
            pagina.Botoes ??= new List<BotaoDTO>();

            return View("Editar", pagina);
        }

        [HttpPost("Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            PaginaDTO model,
            IFormFile BannerFile,
            List<IFormFile> CarrosselFiles,
            string carrosselTitulo)
        {
            if (!ModelState.IsValid)
                return View("Editar", model);

            var paginaExistente = await _paginaService.BuscarPorIdAsync(id);
            if (paginaExistente == null)
                return NotFound();

            bool ehTopico = paginaExistente.CdPai.HasValue;

            // Validar URL somente se alterada
            if (!ehTopico && paginaExistente.Url != model.Url)
            {
                var paginas = await _paginaService.ListarAsync();
                if (paginas.Any(x => x.Url == model.Url && x.Codigo != id))
                {
                    ModelState.AddModelError("Url", "Já existe uma página principal com esta URL.");
                    return View("Editar", model);
                }
            }

            // Upload Banner
            if (BannerFile != null)
            {
                string nome = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nome);

                using var stream = new FileStream(caminho, FileMode.Create);
                await BannerFile.CopyToAsync(stream);

                model.Banner = "/imagens/" + nome;
            }
            else
            {
                model.Banner = paginaExistente.Banner;
            }

            // Slug auto para filhos
            if (model.PaginaFilhos != null)
            {
                foreach (var f in model.PaginaFilhos)
                    if (string.IsNullOrWhiteSpace(f.Url))
                        f.Url = GerarSlug(f.Titulo);
            }

            // Atualizar página
            await _paginaService.AtualizarAsync(id, model, _env.WebRootPath);

            // Carrossel adicional
            if (CarrosselFiles != null && CarrosselFiles.Any())
            {
                var carrossel = await _carrosselService.CriarAsync(
                    new CarrosselDTO
                    {
                        Titulo = string.IsNullOrWhiteSpace(carrosselTitulo)
                            ? "Carrossel adicional"
                            : carrosselTitulo
                    },
                    id);

                int ordem = 1;

                foreach (var file in CarrosselFiles)
                {
                    if (file.Length == 0) continue;

                    string nomeImg = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string caminho = Path.Combine(_env.WebRootPath, "imagens", nomeImg);

                    using var stream = new FileStream(caminho, FileMode.Create);
                    await file.CopyToAsync(stream);

                    await _carrosselImagemService.CriarAsync(
                        new CarrosselImagemDTO
                        {
                            Titulo = file.FileName,
                            UrlImagem = "/imagens/" + nomeImg,
                            Ordem = ordem++
                        },
                        carrossel.Codigo);
                }
            }

            TempData["Mensagem"] = "Página atualizada com sucesso!";

            return ehTopico
                ? RedirectToAction("Gerenciar", new { id = paginaExistente.CdPai })
                : RedirectToAction("Gerenciar", new { id });
        }

        // =======================================================================
        //  EXCLUSÃO
        // =======================================================================

        [HttpGet("Excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);
            if (pagina == null)
                return NotFound();

            return View("Excluir", pagina);
        }

        [HttpPost("Excluir/{id}")]
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

            return ehTopico
                ? RedirectToAction("Gerenciar", new { id = cdPai })
                : RedirectToAction("Listar");
        }

        // =======================================================================
        //  EDITAR CARROSSEL
        // =======================================================================

        [HttpGet("EditarCarrossel/{id}")]
        public async Task<IActionResult> EditarCarrossel(int id)
        {
            var carrossel = await _carrosselService.BuscarPorIdAsync(id);
            if (carrossel == null)
                return NotFound();

            // carrossel.Imagens = await _carrosselImagemService.ListarPorCarrosselAsync(id);

            return View("EditarCarrossel", carrossel);
        }

        [HttpPost("EditarCarrossel/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCarrossel(int id, CarrosselDTO model)
        {
            if (!ModelState.IsValid)
                return View("EditarCarrossel", model);

            await _carrosselService.AtualizarAsync(id, model);

            TempData["Mensagem"] = "Carrossel atualizado com sucesso!";
            return RedirectToAction("Gerenciar", new { id = model.CdPagina });
        }

        [HttpPost("Carrossel/AdicionarImagem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarImagem(int cdCarrossel, string titulo, string descricao, IFormFile imagemFile)
        {
            if (imagemFile != null)
            {
                string nome = Guid.NewGuid() + Path.GetExtension(imagemFile.FileName);
                string caminho = Path.Combine(_env.WebRootPath, "imagens", nome);

                using (var stream = new FileStream(caminho, FileMode.Create))
                    await imagemFile.CopyToAsync(stream);

                var dto = new CarrosselImagemDTO
                {
                    CdCarrossel = cdCarrossel,
                    UrlImagem = "/imagens/" + nome,
                    Titulo = titulo,
                    Descricao = descricao,
                    Ordem = 999 // coloca no final
                };

                await _carrosselImagemService.CriarAsync(dto);
            }

            TempData["Mensagem"] = "Imagem adicionada!";
            return RedirectToAction("EditarCarrossel", new { id = cdCarrossel });
        }

        [HttpPost("Carrossel/EditarImagem/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarImagem(int id, CarrosselImagemDTO model)
        {
            await _carrosselImagemService.AtualizarAsync(id, model);

            TempData["Mensagem"] = "Imagem atualizada!";
            return RedirectToAction("EditarCarrossel", new { id = model.CdCarrossel });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarCarrosselCompleto(CarrosselDTO model)
        {
            if (!ModelState.IsValid)
                return View("EditarCarrossel", model);

            await _carrosselService.SalvarCarrosselCompletoAsync(model);

            TempData["Mensagem"] = "Carrossel atualizado com sucesso!";
            return RedirectToAction("Gerenciar", new { id = model.CdPagina });
        }


        // =======================================================================
        //  EXCLUIR IMAGEM
        // =======================================================================
        [HttpPost("Carrossel/ExcluirImagem/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirImagem(int id, int cdCarrossel)
        {
            await _carrosselImagemService.ExcluirAsync(id);

            TempData["Mensagem"] = "Imagem removida!";
            return RedirectToAction("EditarCarrossel", new { id = cdCarrossel });
        }

        // =======================================================================
        //  EXCLUIR CARROSSEL
        // =======================================================================

        [HttpPost("Carrossel/Excluir/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirCarrossel(int id, int cdPagina)
        {
            await _carrosselService.ExcluirAsync(id);

            TempData["Mensagem"] = "Carrossel excluído!";
            return RedirectToAction("Gerenciar", new { id = cdPagina });
        }


        // =======================================================================
        //  EXIBIR PÚBLICO
        // =======================================================================

        [HttpGet("exibir/{url}", Name = "Pagina_Exibir")]
        public async Task<IActionResult> Exibir(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return NotFound();

            // Busca todas as páginas (como você já fazia)
            var paginas = await _paginaService.ListarAsync();

            // Encontra a página principal pela URL
            var pagina = paginas.FirstOrDefault(p => string.Equals(p.Url, url, StringComparison.OrdinalIgnoreCase)
                                                     && p.Tipo == (int)TipoPagina.Principal);

            if (pagina == null)
                return NotFound();

            // Inicializa coleções
            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.Carrosseis ??= new List<CarrosselDTO>();
            pagina.Botoes ??= new List<BotaoDTO>();

            // 1) Carregar carrosseis da página usando o serviço existente
            try
            {
                var carrosseis = await _carrosselService.ListarPorPaginaAsync(pagina.Codigo);
                if (carrosseis != null && carrosseis.Any())
                {
                    // Para cada carrossel, carregar as imagens
                    foreach (var c in carrosseis)
                    {
                        c.Imagens ??= new List<CarrosselImagemDTO>();
                        var imagens = await _carrosselImagemService.ListarPorCarrosselAsync(c.Codigo);
                        if (imagens != null && imagens.Any())
                        {
                            // garante ordenação por Ordem (opcional)
                            c.Imagens = imagens.OrderBy(i => i.Ordem).ToList();
                        }
                    }

                    pagina.Carrosseis = carrosseis;
                }
            }
            catch
            {
                // opcional: logar erro. Não falhar a renderização inteira.
                pagina.Carrosseis = pagina.Carrosseis ?? new List<CarrosselDTO>();
            }

            // 2) Carregar carrosseis dos tópicos/filhos (se necessário)
            var filhos = paginas.Where(x => x.CdPai == pagina.Codigo).OrderBy(x => x.Ordem).ToList();
            foreach (var f in filhos)
            {
                f.Carrosseis ??= new List<CarrosselDTO>();

                try
                {
                    var carrosseisFilho = await _carrosselService.ListarPorPaginaAsync(f.Codigo);
                    if (carrosseisFilho != null && carrosseisFilho.Any())
                    {
                        foreach (var c in carrosseisFilho)
                        {
                            c.Imagens ??= new List<CarrosselImagemDTO>();
                            var imagens = await _carrosselImagemService.ListarPorCarrosselAsync(c.Codigo); // corrigir nome se diferente
                            if (imagens != null && imagens.Any())
                                c.Imagens = imagens.OrderBy(i => i.Ordem).ToList();
                        }

                        f.Carrosseis = carrosseisFilho;
                    }
                }
                catch
                {
                    f.Carrosseis = f.Carrosseis ?? new List<CarrosselDTO>();
                }
            }

            pagina.PaginaFilhos = filhos;

            return View("Exibir", pagina);
        }


        // =======================================================================
        //  ORDENAÇÃO
        // =======================================================================

        [HttpPost("AtualizarOrdem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarOrdem(int idA, int idB, int paginaId)
        {
            await _paginaService.AtualizarOrdemAsync(idA, idB);
            return RedirectToAction("Gerenciar", new { id = paginaId });
        }

        // =======================================================================
        //  ORDENAÇÃO DE IMAGENS DO CARROSSEL
        // =======================================================================

        [HttpPost("CarrosselImagem/AtualizarOrdem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarOrdemImagem(int idA, int idB, int carrosselId, int paginaId)
        {
            await _carrosselImagemService.AtualizarOrdemAsync(idA, idB);

            TempData["Mensagem"] = "Ordem atualizada!";
            return RedirectToAction("Gerenciar", new { id = paginaId });
        }

        // =======================================================================
        //  GERENCIAR PÁGINA
        // =======================================================================

        [HttpGet("Gerenciar/{id}")]
        public async Task<IActionResult> Gerenciar(int id)
        {
            var pagina = await _paginaService.BuscarPorIdAsync(id);

            if (pagina == null)
                return NotFound();

            pagina.PaginaFilhos ??= new List<PaginaDTO>();
            pagina.Botoes ??= new List<BotaoDTO>();
            pagina.Carrosseis ??= new List<CarrosselDTO>();

            return View("Gerenciar", pagina);
        }

        // =======================================================================
        //  DASHBOARD
        // =======================================================================

        public async Task<IActionResult> Dashboard(string periodo = null, DateTime? inicio = null, DateTime? fim = null)
        {
            if (!Enum.TryParse(periodo, true, out PeriodoRelatorio periodoEnum))
                periodoEnum = PeriodoRelatorio.MesAtual;

            if (string.IsNullOrEmpty(periodo) && !inicio.HasValue && !fim.HasValue)
            {
                ViewBag.PeriodoSelecionado = periodoEnum.ToString();
                return View(null);
            }

            var dados = await _paginaService.ObterDadosDashboardAsync(inicio, fim, periodoEnum);

            ViewBag.PeriodoSelecionado = periodoEnum.ToString();

            return View(dados);
        }

        [HttpPost("DashboardPdf")]
        public async Task<IActionResult> DashboardPdf(string graficoBase64)
        {
            byte[] bytesGrafico = null;

            if (!string.IsNullOrWhiteSpace(graficoBase64))
            {
                graficoBase64 = graficoBase64.Replace("data:image/png;base64,", "");
                bytesGrafico = Convert.FromBase64String(graficoBase64);
            }

            var dados = await _paginaService.ObterDadosDashboardAsync();

            var pdf = _dashboardService.GerarPdf(dados, bytesGrafico);

            return File(pdf, "application/pdf", "Dashboard.pdf");
        }

        [HttpPost("DashboardExcel")]
        public async Task<IActionResult> DashboardExcel()
        {
            // Captura os dados sem precisar de gráfico
            var dados = await _paginaService.ObterDadosDashboardAsync();

            // Chama o serviço que você vai criar (GerarExcel)
            var excel = _dashboardService.GerarExcel(dados);

            return File(
                excel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Dashboard.xlsx"
            );
        }

        // =======================================================================
        //  SLUG
        // =======================================================================

        private string GerarSlug(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return Guid.NewGuid().ToString();

            return titulo
                .ToLowerInvariant()
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => char.IsLetterOrDigit(c) || c == ' ')
                .Aggregate("", (acc, c) => acc + c)
                .Trim()
                .Replace(" ", "-")
                .Trim('-');
        }
    }
}
