using Urll.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ILinksRepository, RedisLinksRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("api/links/{code}", async (string code, ILinksRepository repository) =>
{
    Link? link = await repository.GetOrDefault(code);
    if (link is null)
    {
        return Results.NotFound();
    }

    LinkDto dto = new(link.Created, link.Url, link.Code);
    return Results.Ok(dto);
});

app.MapPost("api/links", async (AddLinkDto dto, ILinksRepository repository) =>
{
    Link link = new(dto.Url, dto.Code);
    bool result = await repository.Add(link);
    if (!result)
    {
        return Results.BadRequest();
    }

    return Results.Ok(link);
});

app.MapGet("{code}", async (string code, ILinksRepository repository) =>
{
    Link? link = await repository.GetOrDefault(code);
    if (link is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(link.Url);
});

app.Run();