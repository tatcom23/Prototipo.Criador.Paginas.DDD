using System.Threading.Tasks;
using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Infrastructure.Repositories;
using Paginas.Infrastructure.Tests.TestHelpers;

namespace Paginas.Infrastructure.Tests
{
    public class PaginaRepositoryTests
    {
        [Fact]
        public async Task AdicionarPaginaComBotao_EObterPorId_DeveRetornarPaginaComBotao()
        {
            // Cria contexto em memória
            var context = InMemoryContextFactory.CreateContext();

            // Cria repositório
            var paginaRepo = new PaginaRepository(context);
            var botaoRepo = new BotaoRepository(context);

            // Cria página
            var pagina = new Pagina("Título Teste", "Conteúdo Teste", "/url-teste", TipoPagina.Principal);

            // Cria botão
            var botao = new Botao("Botao Teste", "https://exemplo.com", TipoBotao.Primario, pagina.Codigo, 1, 1);

            // Adiciona botão à página
            pagina.AdicionarBotao(botao);

            // Adiciona página (que tem o botão dentro)
            await paginaRepo.AdicionarAsync(pagina);
            await paginaRepo.SalvarAlteracoesAsync();

            // Recupera página com botões pelo id
            var resultado = await paginaRepo.ObterPorIdAsync(pagina.Codigo);

            Assert.NotNull(resultado);
            Assert.Equal("Título Teste", resultado.Titulo);

            Assert.NotNull(resultado.Botoes);
            Assert.Single(resultado.Botoes);
            Assert.Equal("Botao Teste", resultado.Botoes[0].Nome);
        }
    }
}

