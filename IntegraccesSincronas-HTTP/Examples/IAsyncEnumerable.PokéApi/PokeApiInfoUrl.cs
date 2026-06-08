namespace IAsyncEnumerable.PokéApi;

public record PokeApiInfoUrl(string Name, string Url);
public record PokeApiResponse<T>(int Count, string? Next, string? Previous, T[] Results);