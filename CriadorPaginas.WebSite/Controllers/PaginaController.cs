using Microsoft.AspNetCore.Mvc;
using Paginas.Domain.Entities;
using Paginas.Domain.Repositories.Interfaces;
using Paginas.Application.DTOs;
using System.Threading.Tasks;

namespace CriadorPaginas.WebSite.Controllers
{
    public class PaginaController : Controller
    {
        private readonly IPaginaAppService _paginaService;

        public PaginaController(IPaginaAppService paginaService)
        {
            _paginaService = paginaService;
        }

        // Exibe lista de páginas
        public async Task<IActionResult> Index()
        {
            var paginas = await _paginaService.ObterTodosAsync();
            return View(paginas); // View recebe uma lista de DTOs
        }

        // Exibe formulário para criar página
        public IActionResult Create()
        {
            return View();
        }

        // Salva página no banco
        [HttpPost]
        public async Task<IActionResult> Create(PaginaDto paginaDto)
        {
            if (ModelState.IsValid)
            {
                await _paginaService.AdicionarAsync(paginaDto);
                return RedirectToAction("Index");
            }
            return View(paginaDto);
        }
    }
}
