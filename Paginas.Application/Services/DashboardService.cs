using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Paginas.Application.DTOs;
using System;
using System.IO;

namespace Paginas.Application.Services
{
    public class DashboardService : IDashboardService
    {
        public byte[] GerarPdf(DashboardViewModel model, byte[] graficoBytes)
        {
            using var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            int y = 40;
            var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
            var fontSubTitle = new XFont("Arial", 14, XFontStyle.Bold);
            var fontText = new XFont("Arial", 12, XFontStyle.Regular);

            // Título
            gfx.DrawString("Dashboard de Páginas Introdutórias", fontTitle, XBrushes.Black,
                new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 50;

            // Total de páginas
            gfx.DrawString($"Total de Páginas Ativas: {model.TotalPaginasAtivas}", fontSubTitle, XBrushes.Blue,
                new XRect(40, y, page.Width, 20), XStringFormats.TopLeft);
            y += 40;

            // Tabela
            gfx.DrawString("Páginas Ativas", fontSubTitle, XBrushes.Black, 40, y);
            y += 25;

            // Cabeçalho da tabela
            gfx.DrawString("Título", fontText, XBrushes.Black, 40, y);
            gfx.DrawString("Data Criação", fontText, XBrushes.Black, 200, y);
            gfx.DrawString("Data Atualização", fontText, XBrushes.Black, 300, y);
            gfx.DrawString("Qtd Tópicos", fontText, XBrushes.Black, 420, y);
            y += 20;

            foreach (var item in model.Tabela)
            {
                gfx.DrawString(item.Titulo, fontText, XBrushes.Black, 40, y);
                gfx.DrawString(item.Criacao.ToString("dd/MM/yyyy"), fontText, XBrushes.Black, 200, y);
                gfx.DrawString(item.Atualizacao?.ToString("dd/MM/yyyy") ?? "-", fontText, XBrushes.Black, 300, y);
                gfx.DrawString(item.QuantidadeTopicos.ToString(), fontText, XBrushes.Black, 420, y);
                y += 20;

                // Se a página encher, criar nova
                if (y > page.Height - 100)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }
            }

            // Gráfico
            if (graficoBytes != null && graficoBytes.Length > 0)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);

                // Margem superior
                double marginTop = 40;
                double marginSides = 50;

                // Título do gráfico
                gfx.DrawString(
                    "Páginas Criadas por Mês",
                    fontSubTitle,
                    XBrushes.Black,
                    new XRect(0, marginTop, page.Width, 30),
                    XStringFormats.TopCenter
                );

                // Calcula posição e dimensões do gráfico
                double chartWidth = (page.Width - 2 * marginSides) * 2.0 / 3.0;   // 2/3 da largura útil
                double chartHeight = page.Height / 3.0;                            // 1/3 da altura da página

                // Centraliza horizontalmente
                double x = (page.Width - chartWidth) / 2;
                double z = marginTop + 40; // posição logo abaixo do título

                using var imgStream = new MemoryStream(graficoBytes);
                var img = XImage.FromStream(() => imgStream);

                // Desenha a imagem centralizada com as dimensões calculadas
                gfx.DrawImage(img, x, z, chartWidth, chartHeight);
            }

            document.Save(stream);
            return stream.ToArray();

        }
    }
}
