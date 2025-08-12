using System;
using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;

public class BotaoTests
{
    [Fact]
    public void Construtor_DeveCriarBotaoComPropriedadesCorretas()
    {
        var botao = new Botao("Nome", "https://link.com", TipoBotao.Primario, 1, 2, 3);

        Assert.Equal("Nome", botao.Nome);
        Assert.Equal("https://link.com", botao.Link);
        Assert.Equal(TipoBotao.Primario, botao.Tipo);
        Assert.Equal(1, botao.CdPaginaIntrodutoria);
        Assert.Equal(2, botao.Linha);
        Assert.Equal(3, botao.Coluna);
        Assert.True(botao.Status);
        Assert.Equal(1, botao.Versao);
        Assert.NotEqual(default, botao.Criacao);
        Assert.Null(botao.Atualizacao);
    }

    [Fact]
    public void Atualizar_DeveAlterarPropriedadesEIncrementarVersao()
    {
        var botao = new Botao("Nome", "https://link.com", TipoBotao.Primario, 1, 2, 3);

        botao.Atualizar("NovoNome", "https://novo-link.com", TipoBotao.Secundario, 4, 5);

        Assert.Equal("NovoNome", botao.Nome);
        Assert.Equal("https://novo-link.com", botao.Link);
        Assert.Equal(TipoBotao.Secundario, botao.Tipo);
        Assert.Equal(4, botao.Linha);
        Assert.Equal(5, botao.Coluna);
        Assert.NotNull(botao.Atualizacao);
        Assert.Equal(2, botao.Versao);
    }

    [Fact]
    public void AtivarEDesativar_DeveAlterarStatusCorretamente()
    {
        var botao = new Botao("Nome", "https://link.com", TipoBotao.Primario, 1, 2, 3);

        botao.Desativar();
        Assert.False(botao.Status);

        botao.Ativar();
        Assert.True(botao.Status);
    }

    [Fact]
    public void Construtor_DeveLancarExcecaoQuandoNomeForNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Botao(null!, "https://link.com", TipoBotao.Primario, 1, 2, 3));
    }

    [Fact]
    public void Construtor_DeveLancarExcecaoQuandoLinkForNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Botao("Nome", null!, TipoBotao.Primario, 1, 2, 3));
    }
}
