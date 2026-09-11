namespace Encurtador.Web.Servicos;

public class ValidadorDeUrl
{
    private const int TamanhoMaximo = 2048;

    private static readonly string[] EsquemasPermitidos = ["http", "https"];

    public ResultadoValidacao Validar(string? urlInformada, string hostDaRequisicao)
    {
        var urlSemEspacos = (urlInformada ?? string.Empty).Trim();

        if (urlSemEspacos.Length == 0)
        {
            return ResultadoValidacao.Rejeitada("Informe uma URL.");
        }

        // ADR-005: allowlist, não blocklist — sem o Contains, TryCreate sozinho aceita javascript:/file: como URI absoluta válida.
        if (!Uri.TryCreate(urlSemEspacos, UriKind.Absolute, out var uri)
            || !EsquemasPermitidos.Contains(uri.Scheme))
        {
            return ResultadoValidacao.Rejeitada("Informe uma URL completa, começando com http:// ou https://");
        }

        if (urlSemEspacos.Length > TamanhoMaximo)
        {
            return ResultadoValidacao.Rejeitada("A URL não pode ter mais de 2048 caracteres.");
        }

        // ADR-005: destino no próprio host cria laço de redirect. Comparação ignora caixa e porta (RN-03).
        if (NormalizarHost(uri.Host) == NormalizarHost(hostDaRequisicao))
        {
            return ResultadoValidacao.Rejeitada("Não é possível encurtar um link do próprio encurtador.");
        }

        return ResultadoValidacao.Aceita(urlSemEspacos);
    }

    private static string NormalizarHost(string host)
    {
        // Limitação conhecida (review T-05, R-01): não trata IPv6 literal entre colchetes (ex.: [::1]:8080) —
        // o ':' do próprio endereço seria cortado como se fosse porta. Sem CA/RN cobrindo IPv6; fora de escopo da demo.
        var indiceDaPorta = host.IndexOf(':');
        return (indiceDaPorta >= 0 ? host[..indiceDaPorta] : host).ToLowerInvariant();
    }
}
