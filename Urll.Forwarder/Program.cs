using Urll.Links.Contracts;
using Urll.Links.Contracts.Dto;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("{code}", async (string code, ILinksClient client) =>
{
    // TODO: Validate code
    try
    {
        LinkDto link = await client.Get(code);
        return Results.Redirect(link.Url);
    }
    catch (Exception ex)
    {
        // TODO: Analyze exception
        Console.WriteLine(ex);
        return Results.NotFound();
    }
});

app.Run();
