using Microsoft.AspNetCore.Mvc;
using Redirect.Application.DTOs;
using Redirect.Application.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CriadorPaginas.WebSite.Controllers
{
    public class RedirectController : Controller
    {
        private readonly IRedirectURLService _redirectService;

        public RedirectController(IRedirectURLService redirectService)
        {
            _redirectService = redirectService;
        }

        // GET: /Redirect
        public async Task<IActionResult> Index()
        {
            var redirects = await _redirectService.ObterTodosAsync();
            return View(redirects);
        }

        // GET: /Redirect/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Redirect/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RedirectURLDTO dto)
        {
            if (ModelState.IsValid)
            {
                // 🔹 Normalização da URL
                dto.UrlAntiga = NormalizeUrl(dto.UrlAntiga);
                dto.UrlNova = NormalizeUrl(dto.UrlNova);

                await _redirectService.AdicionarAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: /Redirect/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var redirects = await _redirectService.ObterTodosAsync();
            var dto = redirects.FirstOrDefault(r => r.Codigo == id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // POST: /Redirect/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RedirectURLDTO dto)
        {
            if (ModelState.IsValid)
            {
                // 🔹 Normalização da URL também ao editar
                dto.UrlAntiga = NormalizeUrl(dto.UrlAntiga);
                dto.UrlNova = NormalizeUrl(dto.UrlNova);

                await _redirectService.AtualizarAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: /Redirect/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            await _redirectService.RemoverAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Método auxiliar para normalizar URLs
        private string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            // Remove domínio se existir e mantém apenas o caminho
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            var path = uri.IsAbsoluteUri ? uri.AbsolutePath : url;

            // Remove barra no final e converte para minúsculas
            return path.TrimEnd('/').ToLower();
        }
    }
}
