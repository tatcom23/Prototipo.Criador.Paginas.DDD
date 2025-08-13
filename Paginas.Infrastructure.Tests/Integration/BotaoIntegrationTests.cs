using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Infrastructure.Data.Context;
using Paginas.Infrastructure.Tests.TestHelpers;

public class BotaoIntegrationTests
{
    [Fact]
    public void CriarBotao_DeveSalvarNoBanco()
    {
        using var context = DbContextFactory.Create();

        // Primeiro precisa de uma página para FK
        var pagina = new Pagina("Página Botão", "Conteúdo", "/url", TipoPagina.Principal);
        context.Paginas.Add(pagina);
        context.SaveChanges();

        var botao = new Botao("Nome Botão", "https://link.com", TipoBotao.Primario, pagina.Codigo, 1, 1);
        context.Botoes.Add(botao);
        context.SaveChanges();

        var botaoDoBanco = context.Botoes.Find(botao.Codigo);
        Assert.NotNull(botaoDoBanco);
        Assert.Equal("Nome Botão", botaoDoBanco.Nome);

        // Limpeza
        context.Botoes.Remove(botaoDoBanco);
        context.Paginas.Remove(pagina);
        context.SaveChanges();
    }
}
