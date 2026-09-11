namespace Encurtador.Web.Servicos;

public class ResultadoValidacao
{
    public bool Sucesso { get; }
    public string? Mensagem { get; }
    public string? UrlValidada { get; }

    private ResultadoValidacao(bool sucesso, string? mensagem, string? urlValidada)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
        UrlValidada = urlValidada;
    }

    public static ResultadoValidacao Aceita(string urlValidada) => new(true, null, urlValidada);

    public static ResultadoValidacao Rejeitada(string mensagem) => new(false, mensagem, null);
}
