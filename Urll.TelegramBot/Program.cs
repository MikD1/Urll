using Refit;
using Urll.Links.Contracts;
using Urll.TelegramBot;

IHost host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        AddLinksClient(context.Configuration, services);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

void AddLinksClient(IConfiguration configuration, IServiceCollection services)
{
    string address = configuration["LinksServiceAddress"]
        ?? throw new Exception("Missing Links Service address");

    services
        .AddRefitClient<ILinksClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(address));
}