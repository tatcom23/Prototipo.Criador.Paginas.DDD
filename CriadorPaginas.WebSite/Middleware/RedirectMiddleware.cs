using Microsoft.AspNetCore.Http;
using Redirect.Application.Services.Interfaces;
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
            var urlAtual = (context.Request.Path.Value ?? string.Empty)
                             .TrimEnd('/')
                             .ToLower();

            var redirect = await redirectService.ObterPorUrlAntigaAsync(urlAtual);

            if (redirect != null && redirect.Ativo)
            {
                // Redireciona para a URL nova já normalizada
                context.Response.Redirect(redirect.UrlNova, permanent: true);
                return;
            }

            await _next(context);
        }
    }
}
