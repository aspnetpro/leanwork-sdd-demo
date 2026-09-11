using Encurtador.Tests.Suporte;
using Encurtador.Web.Dados;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Encurtador.Tests;

public class ResolucaoTests
{
    private static async Task<EncurtadorWebApplicationFactory> CriarFactoryComLinkAsync(
        string codigo = "aB3xK9p", string urlDestino = "https://www.exemplo.com.br")
    {
        var factory = new EncurtadorWebApplicationFactory();

        using var escopo = factory.Services.CreateScope();
        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();

        dbContext.Links.Add(new Link
        {
            Codigo = codigo,
            UrlDestino = urlDestino,
            CriadoEm = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync();

        return factory;
    }

    [Fact]
    public async Task CA_12_CodigoExistente_RetornaTrezentosEDois()
    {
        await using var factory = await CriarFactoryComLinkAsync();
        using var cliente = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        var resposta = await cliente.GetAsync("/aB3xK9p");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, resposta.StatusCode);
        Assert.Equal(new Uri("https://www.exemplo.com.br"), resposta.Headers.Location);
    }

    [Fact]
    public async Task CA_13_CodigoInexistente_RetornaQuatrocentosEQuatro()
    {
        await using var factory = new EncurtadorWebApplicationFactory();
        using var cliente = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        var resposta = await cliente.GetAsync("/zZ9qW1t");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, resposta.StatusCode);
        Assert.Null(resposta.Headers.Location);
    }

    [Fact]
    public async Task CA_14_CodigoComCaixaDiferente_NaoResolve()
    {
        await using var factory = await CriarFactoryComLinkAsync();
        using var cliente = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        var resposta = await cliente.GetAsync("/ab3xk9p");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task CA_15_ResolucaoNaoAlteraORegistro()
    {
        await using var factory = await CriarFactoryComLinkAsync();
        using var cliente = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        await cliente.GetAsync("/aB3xK9p");
        await cliente.GetAsync("/aB3xK9p");
        await cliente.GetAsync("/aB3xK9p");

        using var escopo = factory.Services.CreateScope();
        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();
        var link = await dbContext.Links.SingleAsync(l => l.Codigo == "aB3xK9p");

        Assert.Equal("https://www.exemplo.com.br", link.UrlDestino);
        Assert.Equal(1, await dbContext.Links.CountAsync());
    }
}
