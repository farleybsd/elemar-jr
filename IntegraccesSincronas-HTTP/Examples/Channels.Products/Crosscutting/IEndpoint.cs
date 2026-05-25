namespace Channels.Products.Crosscutting;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
