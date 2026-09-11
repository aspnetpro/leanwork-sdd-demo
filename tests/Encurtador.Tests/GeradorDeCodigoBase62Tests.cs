using System.Text.RegularExpressions;
using Encurtador.Web.Servicos;

namespace Encurtador.Tests;

public partial class GeradorDeCodigoBase62Tests
{
    private readonly GeradorDeCodigoBase62 _gerador = new();

    [Fact]
    public void Gerador_ProduzCodigoDeSeteCaracteresBase62()
    {
        var codigo = _gerador.Gerar();

        Assert.Equal(7, codigo.Length);
        Assert.Matches(AlfabetoBase62(), codigo);
    }

    [Fact]
    public void Gerador_NaoRepeteEmMilGeracoes()
    {
        var codigos = Enumerable.Range(0, 1000).Select(_ => _gerador.Gerar()).ToHashSet();

        Assert.Equal(1000, codigos.Count);
    }

    [GeneratedRegex("^[A-Za-z0-9]{7}$")]
    private static partial Regex AlfabetoBase62();
}
