namespace Encurtador.Web.Servicos;

public class ResultadoEncurtamento
{
    public bool Sucesso { get; }
    public string? Codigo { get; }
    public string? Mensagem { get; }

    private ResultadoEncurtamento(bool sucesso, string? codigo, string? mensagem)
    {
        Sucesso = sucesso;
        Codigo = codigo;
        Mensagem = mensagem;
    }

    public static ResultadoEncurtamento Concluido(string codigo) => new(true, codigo, null);

    public static ResultadoEncurtamento Falha(string mensagem) => new(false, null, mensagem);
}
