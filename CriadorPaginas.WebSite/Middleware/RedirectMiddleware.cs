using Microsoft.AspNetCore.Http;
using Redirect.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Redirect.API.Middleware
{
    public class RedirecionamentoMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirecionamentoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRedirecionamentoOrigemService redirectService)
        {
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";

            var urlAtual = (context.Request.Path.Value ?? string.Empty)
                             .Split('?')[0]
                             .TrimEnd('/')
                             .ToLower();

            var origem = await redirectService.ObterPorUrlOrigemAsync(urlAtual);

            if (origem != null && origem.Ativo)
            {
                var destino = redirectService.SelecionarDestinoValido(origem);

                if (destino != null && destino.Ativo)
                {
                    var novaUrl = destino.UrlDestino?.TrimEnd('/').ToLower();
                    if (string.IsNullOrWhiteSpace(novaUrl) &&
                        !string.Equals(urlAtual, novaUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        await _next(context);
                        return;
                    }

                    // Evita loop
                    if (string.Equals(urlAtual, novaUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        await _next(context);
                        return;
                    }

                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    novaUrl += novaUrl.Contains("?")
                        ? $"&nocache={timestamp}"
                        : $"?nocache={timestamp}";

                    // Redireciona
                    context.Response.Redirect(novaUrl, permanent: false);
                    return;
                }
            }

            await _next(context);
        }
    }
}
