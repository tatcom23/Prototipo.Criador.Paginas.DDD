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
            // 🔹 Cabeçalhos para evitar cache
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";

            var urlAtual = (context.Request.Path.Value ?? string.Empty)
                             .TrimEnd('/')
                             .ToLower();

            var redirect = await redirectService.ObterPorUrlAntigaAsync(urlAtual);

            if (redirect != null && redirect.Ativo)
            {
                // 🔹 Verifica se há data de início e fim definidas no registro
                var agora = DateTime.Now;
                var inicio = redirect.DtInicial; // Ex: DateTime?
                var fim = redirect.DtFinal;       // Ex: DateTime?

                // 🔹 Só redireciona se estiver dentro do intervalo de tempo
                if ((!inicio.HasValue || agora >= inicio.Value) &&
                    (!fim.HasValue || agora <= fim.Value))
                {
                    var novaUrl = redirect.UrlNova;
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
