using System;
using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;

namespace Paginas.Domain.Tests
{
    public class BotaoTests
    {
        [Fact]
        public void Construtor_DeveCriarBotaoComPropriedadesCorretas()
        {
            var botao = new Botao("Nome", "https://link.com", TipoBotao.Primario, cdPaginaIntrodutoria: 1, ordem: 2);

            Assert.Equal("Nome", botao.Nome);
            Assert.Equal("https://link.com", botao.Link);
            Assert.Equal(TipoBotao.Primario, botao.Tipo);
            Assert.Equal(1, botao.CdPaginaIntrodutoria);
            Assert.Equal(2, botao.Ordem);
            Assert.True(botao.Status);
            Assert.NotEqual(default, botao.Criacao);
            Assert.Null(botao.Atualizacao);
        }

        [Fact]
        public void Construtor_ComCodigo_DeveAtribuirCodigo()
        {
            var botao = new Botao(10, "Nome", "https://link.com", TipoBotao.Primario, cdPaginaIntrodutoria: 1, ordem: 5);

            Assert.Equal(10, botao.Codigo);
            Assert.Equal(5, botao.Ordem);
        }

        [Fact]
        public void Atualizar_DeveAlterarPropriedades()
        {
            var botao = new Botao("Nome", "https://link.com", TipoBotao.Primario, cdPaginaIntrodutoria: 1, ordem: 2);

            botao.Atualizar("NovoNome", "https://novo-link.com", TipoBotao.Secundario, ordem: 4);

            Assert.Equal("NovoNome", botao.Nome);
            Assert.Equal("https://novo-link.com", botao.Link);
            Assert.Equal(TipoBotao.Secundario, botao.Tipo);
            Assert.Equal(4, botao.Ordem);
            Assert.NotNull(botao.Atualizacao);
        }

        [Fact]
        public void AtivarEDesativar_DeveAlterarStatusCorretamente()
        {
            var botao = new Botao("Nome", "https://link.com", TipoBotao.Primario, cdPaginaIntrodutoria: 1, ordem: 2);

            botao.Desativar();
            Assert.False(botao.Status);

            botao.Ativar();
            Assert.True(botao.Status);
        }

        [Fact]
        public void Construtor_DeveLancarExcecaoQuandoNomeForNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Botao(null!, "https://link.com", TipoBotao.Primario, cdPaginaIntrodutoria: 1, ordem: 2));
        }

        [Fact]
        public void Construtor_DeveLancarExcecaoQuandoLinkForNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Botao("Nome", null!, TipoBotao.Primario, cdPaginaIntrodutoria: 1, ordem: 2));
        }
    }
}
