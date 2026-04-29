namespace PicPayManager.Crosscutting.Interfaces;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}