using System.Security.Cryptography;

namespace Encurtador.Web.Servicos;

public class GeradorDeCodigoBase62 : IGeradorDeCodigo
{
    private const int TamanhoDoCodigo = 7;

    private const string Alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    // ADR-003: sorteio por índice do alfabeto via RandomNumberGenerator, não módulo sobre bytes — módulo sobre 256 vicia a distribuição.
    public string Gerar()
    {
        return string.Create(TamanhoDoCodigo, 0, (span, _) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = Alfabeto[RandomNumberGenerator.GetInt32(Alfabeto.Length)];
            }
        });
    }
}
