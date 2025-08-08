public class BotaoDTO
{
    public int Codigo { get; set; }
    public string Nome { get; set; }
    public string Link { get; set; }
    public int Linha { get; set; }
    public int Coluna { get; set; }
    public int Tipo { get; set; }
    public bool Status { get; set; }

    public int CdPaginaIntrodutoria { get; set; } // 🔧 ESSENCIAL!
}
