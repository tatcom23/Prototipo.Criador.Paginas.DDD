using System;
using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;

namespace Paginas.Infrastructure.Tests.Entities
{
    public class PaginaTests
    {
        [Fact]
        public void Construtor_DeveCriarPaginaComPropriedadesCorretas()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            Assert.Equal("Título", pagina.Titulo);
            Assert.Equal("Conteúdo", pagina.Conteudo);
            Assert.Equal("/url", pagina.Url);
            Assert.Equal(TipoPagina.Principal, pagina.Tipo);
            Assert.Null(pagina.CdPai);
            Assert.True(pagina.Status);
            Assert.False(pagina.Publicacao);
            Assert.Equal(0, pagina.Ordem);
            Assert.Equal(1, pagina.Versao);
            Assert.NotEqual(default(DateTime), pagina.Criacao);
            Assert.Null(pagina.Atualizacao);
            Assert.NotNull(pagina.Botoes);
            Assert.Empty(pagina.Botoes);
            Assert.NotNull(pagina.PaginaFilhos);
            Assert.Empty(pagina.PaginaFilhos);
        }

        [Fact]
        public void Construtor_DeveAceitarConteudoNuloEConverterHtmlVazioParaNull()
        {
            var paginaComConteudoNulo = new Pagina("Título", null, "/url", TipoPagina.Principal);
            Assert.Null(paginaComConteudoNulo.Conteudo);

            var paginaComHtmlVazio = new Pagina("Título", "<p>&nbsp;</p>", "/url", TipoPagina.Principal);
            // NormalizeHtmlOrNull deve transformar HTML "vazio" em null
            Assert.Null(paginaComHtmlVazio.Conteudo);
        }

        [Fact]
        public void Atualizar_DeveAlterarPropriedadesEIncrementarVersao()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            pagina.Atualizar("Novo Título", "Novo Conteúdo", "/nova-url", TipoPagina.Topico);

            Assert.Equal("Novo Título", pagina.Titulo);
            Assert.Equal("Novo Conteúdo", pagina.Conteudo);
            Assert.Equal("/nova-url", pagina.Url);
            Assert.Equal(TipoPagina.Topico, pagina.Tipo);
            Assert.NotNull(pagina.Atualizacao);
            Assert.Equal(2, pagina.Versao);
        }

        [Fact]
        public void PublicarEDespublicar_DeveAlterarStatusPublicacao()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            pagina.Publicar();
            Assert.True(pagina.Publicacao);

            pagina.Despublicar();
            Assert.False(pagina.Publicacao);
        }

        [Fact]
        public void AtivarEDesativar_DeveAlterarStatus()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            pagina.Desativar();
            Assert.False(pagina.Status);

            pagina.Ativar();
            Assert.True(pagina.Status);
        }

        [Fact]
        public void AdicionarERemoverBotao_DeveModificarListaDeBotoes()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            // Observação: usamos cdPaginaIntrodutoria = 1 apenas para construir o botão.
            // Em cenários reais o EF vai preencher a FK após salvar a página.
            var botao = new Botao("Nome Botão", "https://link.com", TipoBotao.Primario, cdPaginaIntrodutoria: 1, ordem: 1);

            pagina.AdicionarBotao(botao);
            Assert.Single(pagina.Botoes);

            pagina.RemoverBotao(botao);
            Assert.Empty(pagina.Botoes);
        }

        [Fact]
        public void AdicionarBotao_Null_DeveLancarExcecao()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            Assert.Throws<ArgumentNullException>(() => pagina.AdicionarBotao(null!));
        }

        [Fact]
        public void RemoverBotao_Null_DeveLancarExcecao()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            Assert.Throws<ArgumentNullException>(() => pagina.RemoverBotao(null!));
        }

        [Fact]
        public void DefinirBanner_DeveAtribuirEIncrementarVersao()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            pagina.DefinirBanner("  /imagens/banner.png  ");
            Assert.Equal("/imagens/banner.png", pagina.Banner);
            Assert.NotNull(pagina.Atualizacao);
            Assert.Equal(2, pagina.Versao);

            // limpar o banner passando null ou string vazia
            pagina.DefinirBanner(null);
            Assert.Null(pagina.Banner);
            Assert.Equal(3, pagina.Versao);
        }

        [Fact]
        public void AtualizarOrdem_DeveAlterarOrdemEIncrementarVersao()
        {
            var pagina = new Pagina("Título", "Conteúdo", "/url", TipoPagina.Principal);

            pagina.AtualizarOrdem(5);
            Assert.Equal(5, pagina.Ordem);
            Assert.NotNull(pagina.Atualizacao);
            Assert.Equal(2, pagina.Versao);
        }
    }
}
