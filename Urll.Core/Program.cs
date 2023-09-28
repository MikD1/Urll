using StackExchange.Redis;
using Urll.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
await AddRedisConnection(builder.Services);

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
    Link? link = Link.Create(dto.Url, dto.Code, out string[] validationResult);
    if (link is null)
    {
        return Results.BadRequest(validationResult);
    }

    bool result = await repository.Add(link);
    if (!result)
    {
        return Results.BadRequest();
    }

    return Results.Ok(link);
});

app.MapGet("{code}", async (string code, ILinksRepository repository) =>
{
    // TODO: Validate code
    Link? link = await repository.GetOrDefault(code);
    if (link is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(link.Url);
});

app.Run();

async Task AddRedisConnection(IServiceCollection services)
{
    string redisConnectionString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new Exception("Missing Redis connection string");
    services.AddSingleton(await ConnectionMultiplexer.ConnectAsync(redisConnectionString));
    services.AddTransient<ILinksRepository, RedisLinksRepository>();
}