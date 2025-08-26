using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Infrastructure.Data.Context;

public class PaginaIntegrationTests
{
    [Fact]
    public void CriarPagina_DeveSalvarNoBanco()
    {
        using var context = InMemoryContextFactory.CreateContext();

        var pagina = new Pagina("Título Teste", "Conteúdo Teste", "/url-teste", TipoPagina.Principal);
        context.Paginas.Add(pagina);
        context.SaveChanges();

        var paginaDoBanco = context.Paginas.Find(pagina.Codigo);
        Assert.NotNull(paginaDoBanco);
        Assert.Equal("Título Teste", paginaDoBanco.Titulo);
    }

    [Fact]
    public void AtualizarPagina_DeveAlterarPropriedadesNoBanco()
    {
        using var context = InMemoryContextFactory.CreateContext();

        var pagina = new Pagina("Título Inicial", "Conteúdo Inicial", "/url-inicial", TipoPagina.Principal);
        context.Paginas.Add(pagina);
        context.SaveChanges();

        pagina.Atualizar("Novo Título", "Novo Conteúdo", "/nova-url", TipoPagina.Topico);
        context.SaveChanges();

        var paginaAtualizada = context.Paginas.Find(pagina.Codigo);
        Assert.Equal("Novo Título", paginaAtualizada.Titulo);
        Assert.Equal(2, paginaAtualizada.Versao);
    }
}
