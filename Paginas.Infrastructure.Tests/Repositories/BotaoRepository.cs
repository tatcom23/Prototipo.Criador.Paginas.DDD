using System.Threading.Tasks;
using Xunit;
using Paginas.Domain.Entities;
using Paginas.Domain.Enums;
using Paginas.Infrastructure.Repositories;
using Paginas.Infrastructure.Tests.TestHelpers;

namespace Paginas.Infrastructure.Tests
{
    public class BotaoRepositoryTests
    {
        [Fact]
        public async Task AdicionarEObterPorId_DeveRetornarBotao()
        {
            // Cria contexto em memória
            var context = InMemoryContextFactory.CreateContext();

            // Cria repositório usando contexto
            var repo = new BotaoRepository(context);

            // Cria um objeto Botao (usando a sobrecarga que aceita codigo como primeiro argumento)
            var botao = new Botao(
                codigo: 1,
                nome: "Botao Teste",
                link: "https://exemplo.com",
                tipo: TipoBotao.Primario,
                cdPaginaIntrodutoria: 1,
                ordem: 1
            );

            // Adiciona e salva
            await repo.AdicionarAsync(botao);
            await repo.SalvarAlteracoesAsync();

            // Recupera pelo Id
            var resultado = await repo.ObterPorIdAsync(1);

            // Asserts
            Assert.NotNull(resultado);
            Assert.Equal("Botao Teste", resultado!.Nome);
            Assert.Equal(TipoBotao.Primario, resultado.Tipo);
            Assert.Equal("https://exemplo.com", resultado.Link);
        }
    }
}
