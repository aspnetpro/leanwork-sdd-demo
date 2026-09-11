using Encurtador.Web.Dados;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Encurtador.Web.Servicos;

public class ServicoDeEncurtamento(EncurtadorDbContext dbContext, IGeradorDeCodigo geradorDeCodigo)
{
    private const int MaximoDeTentativas = 5;
    private const int SqliteConstraintUnique = 2067; // SQLITE_CONSTRAINT_UNIQUE

    public async Task<ResultadoEncurtamento> EncurtarAsync(string urlValidada)
    {
        for (var tentativa = 0; tentativa < MaximoDeTentativas; tentativa++)
        {
            var link = new Link
            {
                Codigo = geradorDeCodigo.Gerar(),
                UrlDestino = urlValidada,
                CriadoEm = DateTime.UtcNow,
            };

            dbContext.Links.Add(link);

            try
            {
                await dbContext.SaveChangesAsync();
                return ResultadoEncurtamento.Concluido(link.Codigo);
            }
            catch (DbUpdateException excecao) when (EhColisaoDeCodigo(excecao))
            {
                // ADR-003: unicidade é decidida pelo índice único do banco, não por consulta prévia — evita a janela de corrida entre checar e inserir.
                // O change tracker fica com a entrada falha após o SaveChanges; descartar antes do próximo sorteio, senão o próximo tenta gravar as duas.
                dbContext.Entry(link).State = EntityState.Detached;
            }
        }

        return ResultadoEncurtamento.Falha("Não foi possível gerar o link agora. Tente novamente.");
    }

    private static bool EhColisaoDeCodigo(DbUpdateException excecao) =>
        excecao.InnerException is SqliteException { SqliteExtendedErrorCode: SqliteConstraintUnique };
}
