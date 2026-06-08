namespace IAsyncEnumerable.PokéApi.Services;

public interface IPokemonClient
{
    IAsyncEnumerable<PokeApiInfoUrl[]> GetAllPokemonUrls();
}
public class PokemonClient : IPokemonClient
{
    public const int PageSize = 100;
    private readonly HttpClient _httpClient;

    public PokemonClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PokeApi");
    }

    public async IAsyncEnumerable<PokeApiInfoUrl[]> GetAllPokemonUrls()
    {
        await foreach (var pageResults in GetAllPages<PokeApiInfoUrl>("pokemon"))
        {
            yield return pageResults;
        }
    }

    private async IAsyncEnumerable<T[]> GetAllPages<T>(string endpoint)
    {
        int? totalCount = null;
        int currentCount = 0;
        var currentPage = 0;

        do
        {
            var response = (await _httpClient.GetFromJsonAsync<PokeApiResponse<T>>(
                $"{endpoint}?limit={PageSize}&offset={currentPage * PageSize}"))!;

            totalCount = totalCount ?? response.Count;
            currentCount += response.Results.Length;

            yield return response.Results;
            currentPage++;
        }
        while (totalCount > currentCount);
    }
}
