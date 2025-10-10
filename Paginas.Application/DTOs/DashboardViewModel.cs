using System;
using System.Collections.Generic;

namespace Paginas.Application.DTOs
{
    public class DashboardViewModel
    {
        public int TotalPaginasAtivas { get; set; }

        public List<TabelaItem> Tabela { get; set; } = new();
        public List<GraficoItem> Grafico { get; set; } = new();
        public string GraficoBase64 { get; set; }
    }

    public class TabelaItem
    {
        public string Titulo { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Atualizacao { get; set; }
        public int QuantidadeTopicos { get; set; }
    }

    public class GraficoItem
    {
        public string MesAno { get; set; }
        public int Quantidade { get; set; }
    }
}
