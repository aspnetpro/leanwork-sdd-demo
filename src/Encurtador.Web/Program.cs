using Encurtador.Web.Dados;
using Encurtador.Web.Servicos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<EncurtadorDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=shortener.db"));
builder.Services.AddSingleton<IGeradorDeCodigo, GeradorDeCodigoBase62>();
builder.Services.AddSingleton<ValidadorDeUrl>();
builder.Services.AddScoped<ServicoDeEncurtamento>();

var app = builder.Build();

using (var escopoInicializacao = app.Services.CreateScope())
{
    escopoInicializacao.ServiceProvider.GetRequiredService<EncurtadorDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/{codigo}", async (string codigo, EncurtadorDbContext dbContext) =>
{
    var link = await dbContext.Links.SingleOrDefaultAsync(l => l.Codigo == codigo);

    // permanent: false => 302, nunca 301 (ADR-004) — 301 seria cacheado de forma persistente pelo navegador.
    return link is not null
        ? Results.Redirect(link.UrlDestino, permanent: false)
        : Results.NotFound();
});

app.Run();

public partial class Program;
