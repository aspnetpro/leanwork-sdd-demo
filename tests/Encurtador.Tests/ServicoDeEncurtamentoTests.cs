using Encurtador.Tests.Suporte;
using Encurtador.Web.Dados;
using Encurtador.Web.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Encurtador.Tests;

public class ServicoDeEncurtamentoTests
{
    [Fact]
    public async Task CA_08_MesmaUrlEncurtadaDuasVezes_GeraCodigosDistintos()
    {
        await using var factory = new EncurtadorWebApplicationFactory();
        using var escopo = factory.Services.CreateScope();
        var servico = escopo.ServiceProvider.GetRequiredService<ServicoDeEncurtamento>();

        var primeiro = await servico.EncurtarAsync("https://www.exemplo.com.br");
        var segundo = await servico.EncurtarAsync("https://www.exemplo.com.br");

        Assert.True(primeiro.Sucesso);
        Assert.True(segundo.Sucesso);
        Assert.NotEqual(primeiro.Codigo, segundo.Codigo);

        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();
        var destinos = await dbContext.Links
            .Where(link => link.Codigo == primeiro.Codigo || link.Codigo == segundo.Codigo)
            .Select(link => link.UrlDestino)
            .ToListAsync();

        Assert.All(destinos, destino => Assert.Equal("https://www.exemplo.com.br", destino));
    }

    [Fact]
    public async Task CA_09_ColisaoDeCodigo_ResolvidaPorNovoSorteio()
    {
        await using var factory = new EncurtadorWebApplicationFactory();
        await CriarLinkExistente(factory, "aB3xK9p");

        using var escopo = factory.Services.CreateScope();
        var servico = new ServicoDeEncurtamento(
            escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>(),
            new GeradorFalso("aB3xK9p", "nV7zQ1m"));

        var resultado = await servico.EncurtarAsync("https://www.exemplo.com.br");

        Assert.True(resultado.Sucesso);
        Assert.Equal("nV7zQ1m", resultado.Codigo);

        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();
        Assert.Equal(2, await dbContext.Links.CountAsync());
    }

    [Fact]
    public async Task CA_10_TentativasEsgotadas_FalhaSemGravar()
    {
        await using var factory = new EncurtadorWebApplicationFactory();
        await CriarLinkExistente(factory, "aB3xK9p");

        using var escopo = factory.Services.CreateScope();
        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();
        var servico = new ServicoDeEncurtamento(dbContext, new GeradorFalso("aB3xK9p"));

        var resultado = await servico.EncurtarAsync("https://www.exemplo.com.br");

        Assert.False(resultado.Sucesso);
        Assert.Equal("Não foi possível gerar o link agora. Tente novamente.", resultado.Mensagem);
        Assert.Equal(1, await dbContext.Links.CountAsync());
    }

    private static async Task CriarLinkExistente(EncurtadorWebApplicationFactory factory, string codigo)
    {
        using var escopo = factory.Services.CreateScope();
        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();

        dbContext.Links.Add(new Link
        {
            Codigo = codigo,
            UrlDestino = "https://existente.com.br",
            CriadoEm = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync();
    }
}
