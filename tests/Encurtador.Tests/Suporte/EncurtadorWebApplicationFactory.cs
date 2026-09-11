using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace Encurtador.Tests.Suporte;

// Cada instância aponta para um arquivo SQLite isolado (ADR-008), reutilizada por T-07/T-08/T-09.
public class EncurtadorWebApplicationFactory(string? caminhoDoBanco = null) : WebApplicationFactory<Program>
{
    private readonly bool _bancoGeradoPelaFactory = caminhoDoBanco is null;

    public string CaminhoDoBanco { get; } =
        caminhoDoBanco ?? Path.Combine(Path.GetTempPath(), $"encurtador-teste-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", $"Data Source={CaminhoDoBanco}");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // Microsoft.Data.Sqlite mantém a conexão em pool após o Dispose do DbContext,
        // segurando o handle do arquivo — sem isto o File.Delete abaixo falha.
        SqliteConnection.ClearAllPools();

        if (_bancoGeradoPelaFactory && File.Exists(CaminhoDoBanco))
        {
            File.Delete(CaminhoDoBanco);
        }
    }
}
