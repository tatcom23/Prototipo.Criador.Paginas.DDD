using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  // Para [Required], ErrorMessage
using Microsoft.AspNetCore.Http;            // Para IFormFile
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

        public string Banner { get; set; }
       // public IFormFile BannerFile { get; set; }  // ✅ Upload de arquivo

        public List<BotaoDTO> Botoes { get; set; } = new();
        public List<TopicoDTO> Topicos { get; set; } = new();

        public int? CdPai { get; set; }  // usado em edição de tópico
    }

    public class BotaoDTO
    {
        [Required(ErrorMessage = "O nome do botão é obrigatório.")]
        public string Nome { get; set; }

        public string Link { get; set; }  // Pode ser manual ou gerado com upload

        public int Linha { get; set; }
        public int Coluna { get; set; }

      //  public IFormFile Arquivo { get; set; }  // ✅ Upload de arquivo opcional
    }

    public class TopicoDTO
    {
        [Required(ErrorMessage = "O título do tópico é obrigatório.")]
        public string Titulo { get; set; }

        public string Conteudo { get; set; }
        public string Url { get; set; }
        public string Banner { get; set; }

        public List<BotaoDTO> Botoes { get; set; } = new(); // Corrigido nome da classe
    }
}
