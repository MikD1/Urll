using Refit;
using Urll.Links.Contracts;
using Urll.Links.Contracts.Dto;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
AddLinksClient(builder);

WebApplication app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("{code}", async (string code, ILinksClient client) =>
{
    IApiResponse<LinkDto> response = await client.Get(code);
    if (!response.IsSuccessStatusCode)
    {
        return Results.StatusCode((int)response.StatusCode);
    }

    LinkDto? link = response.Content;
    if (link is null)
    {
        return Results.Problem("Received empty content");
    }

    return Results.Redirect(link.Url);
});

app.Run();

void AddLinksClient(WebApplicationBuilder builder)
{
    string address = builder.Configuration["LinksServiceAddress"]
        ?? throw new Exception("Missing Links Service address");

    builder.Services
        .AddRefitClient<ILinksClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(address));
}
