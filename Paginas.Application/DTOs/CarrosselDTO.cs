using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Paginas.Application.DTOs
{
    public class CarrosselDTO
    {
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O título do carrossel é obrigatório.")]
        public string Titulo { get; set; }

        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Alteracao { get; set; }

        // Relacionamento
        public int CdPagina { get; set; }
        public List<CarrosselImagemDTO> Imagens { get; set; } = new List<CarrosselImagemDTO>();
    }
}
