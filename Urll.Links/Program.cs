using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Urll.Links;
using Urll.Links.Contracts.Dto;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
await AddRedisConnection(builder);

WebApplication app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("api/links", async (ILinksRepository repository) =>
{
    IReadOnlyCollection<Link> links = await repository.GetAll();
    List<LinkDto> dto = links
        .Select(x => new LinkDto(x.Created, x.Url, x.Code))
        .ToList();
    return Results.Ok(dto);
});

app.MapGet("api/links/{code}", async (string code, ILinksRepository repository) =>
{
    // TODO: Validate code
    Link? link = await repository.GetOrDefault(code);
    if (link is null)
    {
        return Results.NotFound();
    }

    LinkDto dto = new(link.Created, link.Url, link.Code);
    return Results.Ok(dto);
});

app.MapPost("api/links", async ([FromBody] LinkAddDto dto, ILinksRepository repository) =>
{
    if (dto.Code is null)
    {
        long id = await repository.GetNextId();
        IdEncoder encoder = new();
        string code = encoder.Encode((int)id);
        dto = dto with { Code = code };
    }

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

app.MapDelete("api/links/{code}", async (string code, ILinksRepository repository) =>
{
    bool result = await repository.Delete(code);
    if (!result)
    {
        return Results.BadRequest();
    }

    return Results.NoContent();
});

app.Run();

async Task AddRedisConnection(WebApplicationBuilder builder)
{
    string redisConnectionString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new Exception("Missing Redis connection string");
    builder.Services
        .AddSingleton(await ConnectionMultiplexer.ConnectAsync(redisConnectionString))
        .AddTransient<ILinksRepository, RedisLinksRepository>();
}
