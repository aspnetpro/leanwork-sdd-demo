using Encurtador.Tests.Suporte;
using Encurtador.Web.Dados;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Encurtador.Tests;

public class InicializacaoTests
{
    [Fact]
    public async Task CA_16_PrimeiraExecucao_CriaBancoAutomaticamente()
    {
        await using var factory = new EncurtadorWebApplicationFactory();

        Assert.False(File.Exists(factory.CaminhoDoBanco));

        using var escopo = factory.Services.CreateScope();
        var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();

        dbContext.Links.Add(new Link
        {
            Codigo = "aB3xK9p",
            UrlDestino = "https://exemplo.com.br",
            CriadoEm = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync();

        Assert.True(File.Exists(factory.CaminhoDoBanco));
        Assert.Equal(1, await dbContext.Links.CountAsync());
    }

    [Fact]
    public async Task SegundaInicializacao_ComBancoExistente_NaoGeraErroNemDuplicaSchema()
    {
        var caminhoDoBanco = Path.Combine(Path.GetTempPath(), $"encurtador-teste-{Guid.NewGuid():N}.db");

        try
        {
            await using (var primeiraFactory = new EncurtadorWebApplicationFactory(caminhoDoBanco))
            {
                _ = primeiraFactory.Services;
            }

            await using var segundaFactory = new EncurtadorWebApplicationFactory(caminhoDoBanco);
            using var escopo = segundaFactory.Services.CreateScope();
            var dbContext = escopo.ServiceProvider.GetRequiredService<EncurtadorDbContext>();

            dbContext.Links.Add(new Link
            {
                Codigo = "zZ9pK3b",
                UrlDestino = "https://exemplo.com.br",
                CriadoEm = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();

            Assert.Equal(1, await dbContext.Links.CountAsync());
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (File.Exists(caminhoDoBanco))
            {
                File.Delete(caminhoDoBanco);
            }
        }
    }
}
