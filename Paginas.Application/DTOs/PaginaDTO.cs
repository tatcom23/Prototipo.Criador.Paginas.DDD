using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Paginas.Domain.Entities;

namespace Paginas.Application.DTOs
{
    public class PaginaDTO
    {
        public int? Codigo { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        public string Titulo { get; set; }

        public string Conteudo { get; set; }
        public string Url { get; set; }

        public string Banner { get; set; }  // Caminho ou nome do arquivo
        public byte[] BannerBytes { get; set; }  // Conteúdo do arquivo (se necessário)

        public List<BotaoDTO> Botoes { get; set; } = new();
        public List<TopicoDTO> Topicos { get; set; } = new();
        public int? CdPai { get; set; }
    }

    public class BotaoDTO
    {
        [Required(ErrorMessage = "O nome do botão é obrigatório.")]
        public string Nome { get; set; }
        public string Link { get; set; }
        public int Linha { get; set; }
        public int Coluna { get; set; }
        public byte[] ArquivoBytes { get; set; }  // Conteúdo do arquivo
    }

    public class TopicoDTO
    {
        [Required(ErrorMessage = "O título do tópico é obrigatório.")]
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string Url { get; set; }
        public string Banner { get; set; }
        public List<BotaoDTO> Botoes { get; set; } = new();
    }
}
