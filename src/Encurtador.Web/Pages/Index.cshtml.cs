using Encurtador.Web.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Encurtador.Web.Pages;

public class IndexModel(ValidadorDeUrl validador, ServicoDeEncurtamento servicoDeEncurtamento) : PageModel
{
    [BindProperty]
    public string? UrlInformada { get; set; }

    public string? MensagemDeErro { get; private set; }

    public string? UrlCurta { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var resultadoValidacao = validador.Validar(UrlInformada, Request.Host.Host);

        if (!resultadoValidacao.Sucesso)
        {
            MensagemDeErro = resultadoValidacao.Mensagem;
            return Page();
        }

        var resultadoEncurtamento = await servicoDeEncurtamento.EncurtarAsync(resultadoValidacao.UrlValidada!);

        if (!resultadoEncurtamento.Sucesso)
        {
            MensagemDeErro = resultadoEncurtamento.Mensagem;
            return Page();
        }

        UrlCurta = $"{Request.Scheme}://{Request.Host}/{resultadoEncurtamento.Codigo}";
        return Page();
    }
}
