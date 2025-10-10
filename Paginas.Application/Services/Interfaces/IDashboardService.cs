using Paginas.Application.DTOs;

namespace Paginas.Application.Services
{
    public interface IDashboardService
    {
        byte[] GerarPdf(DashboardViewModel model, byte[] graficoBytes);
    }
}
