using System.Text.RegularExpressions;
using Encurtador.Tests.Suporte;
using Encurtador.Web.Dados;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Encurtador.Tests;

public class EncurtamentoWebTests
{
    [Fact]
    public async Task CA_01_UrlValida_CriaLinkEExibeUrlCurtaCompleta()
    {
        await using var factory = new EncurtadorWebApplicationFactory();
        using var cliente = factory.CreateClient();

        var token = await ObterAntiforgeryTokenAsync(cliente);

        var resposta = await cliente.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["UrlInformada"] = "https://www.exemplo.com.br/artigos/2026/engenharia-de-software",
            ["__RequestVerificationToken"] = token,
        }));

        Assert.Equal(System.Net.HttpStatusCode.OK, resposta.StatusCode);

        var html = await resposta.Content.ReadAsStringAsync();
        var correspondencia = Regex.Match(html, @"id=""url-curta"" value=""(https?://[^/""]+/([A-Za-z0-9]{7}))""");
        Assert.True(correspondencia.Success, "A resposta deveria conter a URL curta completa.");

        var codigo = correspondencia.Groups[2].Value;

        using var escopo = factory.Services.CreateScope();
        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();
        var link = await dbContext.Links.SingleAsync(l => l.Codigo == codigo);

        Assert.Equal("https://www.exemplo.com.br/artigos/2026/engenharia-de-software", link.UrlDestino);
        Assert.Matches("^[A-Za-z0-9]{7}$", link.Codigo);
        Assert.Equal(DateTimeKind.Utc, link.CriadoEm.Kind);
    }

    [Fact]
    public async Task CA_11_UrlInvalida_PreservaEntradaEExibeMensagem()
    {
        await using var factory = new EncurtadorWebApplicationFactory();
        using var cliente = factory.CreateClient();

        var token = await ObterAntiforgeryTokenAsync(cliente);

        var resposta = await cliente.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["UrlInformada"] = "google.com",
            ["__RequestVerificationToken"] = token,
        }));

        Assert.Equal(System.Net.HttpStatusCode.OK, resposta.StatusCode);

        var html = System.Net.WebUtility.HtmlDecode(await resposta.Content.ReadAsStringAsync());
        Assert.Contains("value=\"google.com\"", html);
        Assert.Contains("Informe uma URL completa, começando com http:// ou https://", html);
    }

    private static async Task<string> ObterAntiforgeryTokenAsync(HttpClient cliente)
    {
        var html = await (await cliente.GetAsync("/")).Content.ReadAsStringAsync();
        var correspondencia = Regex.Match(
            html,
            @"name=""__RequestVerificationToken""\s+type=""hidden""\s+value=""([^""]+)""");

        Assert.True(correspondencia.Success, "Token antiforgery não encontrado na página.");
        return correspondencia.Groups[1].Value;
    }
}
