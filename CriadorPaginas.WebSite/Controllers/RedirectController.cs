using Microsoft.AspNetCore.Mvc;
using Redirect.Application.DTOs;
using Redirect.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CriadorPaginas.WebSite.Controllers
{
    public class RedirectController : Controller
    {
        private readonly IRedirecionamentoOrigemService _origemService;
        private readonly IRedirecionamentoDestinoService _destinoService;

        public RedirectController(
            IRedirecionamentoOrigemService origemService,
            IRedirecionamentoDestinoService destinoService)
        {
            _origemService = origemService;
            _destinoService = destinoService;
        }

        // 🔹 LISTAGEM DE ORIGENS
        public async Task<IActionResult> Index()
        {
            var origens = await _origemService.ObterTodosAsync();
            return View(origens);
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

        // 🔹 EDITAR ORIGEM COMPLETA
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

        // 🔹 EDITAR DESTINO INDIVIDUAL
        public async Task<IActionResult> EditDestino(int id)
        {
            var destino = await _destinoService.ObterPorIdAsync(id);
            if (destino == null)
                return NotFound();

            return View(destino); // View "EditDestino.cshtml"
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDestino(RedirecionamentoDestinoDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _destinoService.AtualizarAsync(dto);
            TempData["SuccessMessage"] = "Destino atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // 🔹 EXCLUIR ORIGEM (ETAPA 1 - CONFIRMAÇÃO)
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _origemService.ObterPorIdAsync(id);
            if (dto == null)
                return NotFound();

            return View("ConfirmDeleteOrigem", dto);
        }

        // 🔹 EXCLUIR ORIGEM (ETAPA 2 - CONFIRMADO)
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

        // 🔹 EXCLUIR DESTINO (ETAPA 1 - CONFIRMAÇÃO)
        public async Task<IActionResult> DeleteDestino(int id)
        {
            var destino = await _destinoService.ObterPorIdAsync(id);
            if (destino == null)
                return NotFound();

            return View("ConfirmDeleteDestino", destino);
        }

        // 🔹 EXCLUIR DESTINO (ETAPA 2 - CONFIRMADO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDestinoConfirmed(int id)
        {
            try
            {
                var destino = await _destinoService.ObterPorIdAsync(id);
                if (destino == null)
                    return NotFound();

                await _destinoService.RemoverAsync(id);
                TempData["SuccessMessage"] = "Destino excluído com sucesso.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao excluir destino: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
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
