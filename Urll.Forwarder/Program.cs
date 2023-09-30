using StackExchange.Redis;
using Urll.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
await AddRedisConnection(builder.Services);

WebApplication app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

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