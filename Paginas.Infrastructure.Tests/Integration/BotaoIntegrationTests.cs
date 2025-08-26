using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Infrastructure.Data.Context;

public class BotaoIntegrationTests
{
    [Fact]
    public void CriarBotao_DeveSalvarNoBanco()
    {
        using var context = InMemoryContextFactory.CreateContext();

        var pagina = new Pagina("Página Botão", "Conteúdo", "/url", TipoPagina.Principal);
        context.Paginas.Add(pagina);
        context.SaveChanges();

        var botao = new Botao("Nome Botão", "https://link.com", TipoBotao.Primario, pagina.Codigo, 1);
        context.Botoes.Add(botao);
        context.SaveChanges();

        var botaoDoBanco = context.Botoes.Find(botao.Codigo);
        Assert.NotNull(botaoDoBanco);
        Assert.Equal("Nome Botão", botaoDoBanco.Nome);
    }
}
