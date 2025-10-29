using Microsoft.AspNetCore.Http;
using Redirect.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Redirect.API.Middleware
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRedirectURLService redirectService)
        {
            // 🔹 Adiciona cabeçalhos HTTP para evitar cache em todas as respostas
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";

            var urlAtual = (context.Request.Path.Value ?? string.Empty)
                             .TrimEnd('/')
                             .ToLower();

            var redirect = await redirectService.ObterPorUrlAntigaAsync(urlAtual);

            if (redirect != null && redirect.Ativo)
            {
                // 🔹 Cria a URL de destino garantindo que o cache seja sempre ignorado
                var novaUrl = redirect.UrlNova;

                // Adiciona um parâmetro nocache com timestamp para forçar nova requisição
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (!novaUrl.Contains("?"))
                    novaUrl += $"?nocache={timestamp}";
                else
                    novaUrl += $"&nocache={timestamp}";

                // 🔹 Redireciona temporariamente (não cacheia o redirecionamento)
                context.Response.Redirect(novaUrl, permanent: false);
                return;
            }

            await _next(context);
        }
    }
}
