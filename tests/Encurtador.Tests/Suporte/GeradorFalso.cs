using Encurtador.Web.Servicos;

namespace Encurtador.Tests.Suporte;

// Devolve os códigos informados em ordem; esgotada a fila, repete o último — cobre tanto
// "colide uma vez e depois sorteia um código válido" (CA-09) quanto "colide sempre" (CA-10).
public class GeradorFalso(params string[] codigos) : IGeradorDeCodigo
{
    private int _indice;

    public string Gerar()
    {
        var codigo = codigos[Math.Min(_indice, codigos.Length - 1)];
        _indice++;
        return codigo;
    }
}
