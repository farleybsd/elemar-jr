using IAsyncEnumerable.PokéApi.BackGroundServices;
using IAsyncEnumerable.PokéApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseConsoleLifetime();

builder.Services.AddHostedService<ConsoleRunner>();

builder.Services.AddHttpClient("PokeApi", client =>
{
    client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<IPokemonClient, PokemonClient>();

var app = builder.Build();

app.UseHttpsRedirection();

await app.RunAsync();