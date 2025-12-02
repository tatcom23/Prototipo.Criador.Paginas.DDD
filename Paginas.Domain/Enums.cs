using System.ComponentModel.DataAnnotations;

namespace Paginas.Domain.Enums
{
    public enum TipoBotao
    {
        [Display(Name = "Não Definido")]
        NaoDefinido = 0,

        [Display(Name = "Topo da Página")]
        Topo = 1,

        [Display(Name = "Final da Página")]
        Final = 2
    }

public enum TipoPagina
    {
        [Display(Name = "Principal")]
        Principal = 1,

        [Display(Name = "Tópico")]
        Topico = 2

    }

    public enum PeriodoRelatorio
    {
        [Display(Name = "Mês atual")]
        MesAtual,

        [Display(Name = "Último mês")]
        UltimoMes,

        [Display(Name = "Último trimestre")]
        UltimoSemestre,
        
        [Display(Name = "Último ano")]
        UltimoAno,

        [Display(Name = "Todos")]
        Todos,

        [Display(Name = "Personalizado")]
        Personalizado
    }

}