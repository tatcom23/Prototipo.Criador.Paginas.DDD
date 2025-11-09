using System;
using System.ComponentModel.DataAnnotations;

namespace Paginas.Application.DTOs
{
    public class CarrosselImagemDTO
    {
        public int Codigo { get; set; }

        [Required(ErrorMessage = "A URL da imagem é obrigatória.")]
        public string UrlImagem { get; set; }

        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Alteracao { get; set; }
        public bool Excluir { get; set; }

        // Relacionamento
        public int CdCarrossel { get; set; }
    }
}
