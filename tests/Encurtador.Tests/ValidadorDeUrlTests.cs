using Encurtador.Web.Servicos;

namespace Encurtador.Tests;

public class ValidadorDeUrlTests
{
    private const string HostDaRequisicao = "curto.local";

    private readonly ValidadorDeUrl _validador = new();

    [Theory]
    [InlineData("google.com")]
    [InlineData("www.exemplo.com.br/pagina")]
    [InlineData("javascript:alert(1)")]
    [InlineData("data:text/html,<h1>oi</h1>")]
    [InlineData("file:///etc/passwd")]
    [InlineData("ftp://exemplo.com.br/arquivo.zip")]
    [InlineData("//exemplo.com.br")]
    public void CA_02_UrlComEsquemaAusenteOuNaoPermitido_DeveSerRejeitada(string entrada)
    {
        var resultado = _validador.Validar(entrada, HostDaRequisicao);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Informe uma URL completa, começando com http:// ou https://", resultado.Mensagem);
    }

    [Fact]
    public void CA_03_UrlAcimaDoLimite_DeveSerRejeitada()
    {
        var url = ConstruirUrlComTamanho(2049);

        var resultado = _validador.Validar(url, HostDaRequisicao);

        Assert.False(resultado.Sucesso);
        Assert.Equal("A URL não pode ter mais de 2048 caracteres.", resultado.Mensagem);
    }

    [Fact]
    public void CA_04_UrlNoLimiteExato_DeveSerAceita()
    {
        var url = ConstruirUrlComTamanho(2048);

        var resultado = _validador.Validar(url, HostDaRequisicao);

        Assert.True(resultado.Sucesso);
        Assert.Equal(url, resultado.UrlValidada);
    }

    [Fact]
    public void CA_04_UrlNoLimiteExatoComEspacosNasExtremidades_DeveSerAceita()
    {
        var urlSemEspacos = ConstruirUrlComTamanho(2048);

        var resultado = _validador.Validar($"  {urlSemEspacos}  ", HostDaRequisicao);

        Assert.True(resultado.Sucesso);
        Assert.Equal(urlSemEspacos, resultado.UrlValidada);
    }

    [Fact]
    public void CA_05_DestinoNoProprioHost_DeveSerRejeitado()
    {
        var resultado = _validador.Validar("https://curto.local/aB3xK9p", HostDaRequisicao);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Não é possível encurtar um link do próprio encurtador.", resultado.Mensagem);
    }

    [Fact]
    public void CA_05_DestinoNoProprioHostComCaixaDiferente_DeveSerRejeitado()
    {
        var resultado = _validador.Validar("https://CURTO.local/aB3xK9p", HostDaRequisicao);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Não é possível encurtar um link do próprio encurtador.", resultado.Mensagem);
    }

    [Fact]
    public void CA_05_DestinoNoProprioHostComPortaDiferente_DeveSerRejeitado()
    {
        var resultado = _validador.Validar("https://curto.local:8080/aB3xK9p", HostDaRequisicao);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Não é possível encurtar um link do próprio encurtador.", resultado.Mensagem);
    }

    [Fact]
    public void CA_17_EntradaComMultiplasViolacoes_ExibeApenasAPrimeira()
    {
        var url = "javascript:" + new string('a', 3000);

        var resultado = _validador.Validar(url, HostDaRequisicao);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Informe uma URL completa, começando com http:// ou https://", resultado.Mensagem);
    }

    [Fact]
    public void CA_06_CampoApenasComEspacos_DeveSerRejeitado()
    {
        var resultado = _validador.Validar("   ", HostDaRequisicao);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Informe uma URL.", resultado.Mensagem);
    }

    [Fact]
    public void CA_07_EspacosNasExtremidades_SaoRemovidos()
    {
        var resultado = _validador.Validar("  https://www.exemplo.com.br  ", HostDaRequisicao);

        Assert.True(resultado.Sucesso);
        Assert.Equal("https://www.exemplo.com.br", resultado.UrlValidada);
    }

    private static string ConstruirUrlComTamanho(int tamanho)
    {
        const string prefixo = "https://exemplo.com.br/";
        var preenchimento = new string('a', tamanho - prefixo.Length);
        var url = prefixo + preenchimento;

        Assert.Equal(tamanho, url.Length);

        return url;
    }
}
