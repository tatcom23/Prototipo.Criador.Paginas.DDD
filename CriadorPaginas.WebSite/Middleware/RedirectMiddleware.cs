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
                             .TrimEnd('/')
                             .ToLower();

            var origem = await redirectService.ObterPorUrlOrigemAsync(urlAtual);

            if (origem != null && origem.Ativo)
            {
                var destino = redirectService.SelecionarDestinoValido(origem);

                if (destino != null)
                {
                    var novaUrl = destino.UrlDestino;
                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    if (!novaUrl.Contains("?"))
                        novaUrl += $"?nocache={timestamp}";
                    else
                        novaUrl += $"&nocache={timestamp}";

                    context.Response.Redirect(novaUrl, permanent: false);
                    return;
                }
            }

            await _next(context);
        }
    }
}
