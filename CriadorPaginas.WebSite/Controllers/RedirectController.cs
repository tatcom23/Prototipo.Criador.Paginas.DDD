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
        private readonly IRedirecionamentoOrigemService _origemService;

        public RedirectController(IRedirecionamentoOrigemService origemService)
        {
            _origemService = origemService;
        }

        // 🔹 LISTAGEM DE ORIGENS (com paginação)
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var (itens, total) = await _origemService.ObterPaginadoAsync(page, pageSize);
            ViewBag.Total = total;
            return View(itens);
        }


        // 🔹 CRIAR NOVO
        public IActionResult Create()
        {
            return View(new RedirecionamentoOrigemDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RedirecionamentoOrigemDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            dto.UrlOrigem = NormalizeUrl(dto.UrlOrigem);
            dto.DtRedirecionamento = DateTime.Now;

            await _origemService.AdicionarAsync(dto);
            TempData["SuccessMessage"] = "Redirecionamento criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // 🔹 EDITAR ORIGEM E DESTINOS
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _origemService.ObterPorIdAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RedirecionamentoOrigemDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            dto.UrlOrigem = NormalizeUrl(dto.UrlOrigem);
            dto.DtAtualizacao = DateTime.Now;

            await _origemService.AtualizarAsync(dto);
            TempData["SuccessMessage"] = "Redirecionamento atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // 🔹 EXCLUIR ORIGEM (ETAPA 1)
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _origemService.ObterPorIdAsync(id);
            if (dto == null)
                return NotFound();

            return View("ConfirmDeleteOrigem", dto);
        }

        // 🔹 EXCLUIR ORIGEM (ETAPA 2)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _origemService.RemoverAsync(id);
                TempData["SuccessMessage"] = "Origem e destinos excluídos com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao excluir origem: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // 🔹 EXCLUIR DESTINO (ETAPA 1)
        public async Task<IActionResult> DeleteDestino(int id)
        {
            var origens = await _origemService.ObterTodosAsync();
            var origem = origens?.FirstOrDefault(o => o.Destinos?.Any(d => d.Codigo == id) == true);

            if (origem == null)
                return NotFound();

            var destino = origem.Destinos?.FirstOrDefault(d => d.Codigo == id);
            if (destino == null)
                return NotFound();

            return View("ConfirmDeleteDestino", destino);
        }

        // 🔹 EXCLUIR DESTINO (ETAPA 2)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDestinoConfirmed(int id)
        {
            try
            {
                var origem = (await _origemService.ObterTodosAsync())
                    .FirstOrDefault(o => o.Destinos?.Any(d => d.Codigo == id) == true);

                if (origem == null)
                    return NotFound();

                origem.Destinos?.RemoveAll(d => d.Codigo == id);

                await _origemService.AtualizarAsync(origem);
                TempData["SuccessMessage"] = "Destino excluído com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao excluir destino: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // 🔹 Normaliza URLs
        private string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                var path = uri.IsAbsoluteUri ? uri.AbsolutePath : url;
                return path.TrimEnd('/').ToLowerInvariant();
            }
            catch
            {
                return url.TrimEnd('/').ToLowerInvariant();
            }
        }
    }
}
