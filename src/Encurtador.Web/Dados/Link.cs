namespace Encurtador.Web.Dados;

public class Link
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string UrlDestino { get; set; }
    public DateTime CriadoEm { get; set; }
}
