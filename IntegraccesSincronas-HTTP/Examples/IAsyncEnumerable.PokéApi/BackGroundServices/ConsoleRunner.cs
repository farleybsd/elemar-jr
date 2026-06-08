using IAsyncEnumerable.PokéApi.Services;

namespace IAsyncEnumerable.PokéApi.BackGroundServices;

public class ConsoleRunner : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<ConsoleRunner> _logger;
    private readonly IPokemonClient _pokemonClient;

    public ConsoleRunner(IHostApplicationLifetime hostApplicationLifetime, ILogger<ConsoleRunner> logger, IPokemonClient pokemonClient)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
        _pokemonClient = pokemonClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var allPokemon = new List<PokeApiInfoUrl>();

        await foreach (var pokemonPage in _pokemonClient.GetAllPokemonUrls())
        {
            allPokemon.AddRange(pokemonPage);
        }
    }
}