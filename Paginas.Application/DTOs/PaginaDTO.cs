using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Paginas.Application.DTOs
{
    public class PaginaDTO
    {
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        public string Titulo { get; set; }

        public string Conteudo { get; set; }
        public string Url { get; set; }
        public int Tipo { get; set; }
        public int? CdPai { get; set; }
        public bool Publicacao { get; set; }
        public bool Status { get; set; }
        public int Ordem { get; set; }
        public int Versao { get; set; }
        public string Banner { get; set; }
        public byte[] BannerBytes { get; set; }
        public DateTime Criacao { get; set; }
        public DateTime? Atualizacao { get; set; }

        public List<BotaoDTO> Botoes { get; set; } = new();
        public List<PaginaDTO> PaginaFilhos { get; set; } = new();

        // ✅ Adicionadas propriedades para Carrosséis
        public List<CarrosselDTO> Carrosseis { get; set; } = new List<CarrosselDTO>();
    }
}
