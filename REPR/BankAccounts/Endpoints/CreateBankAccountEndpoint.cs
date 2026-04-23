using REPR.interfaces;

namespace REPR.BankAccounts.Endpoints;

public record struct CreateBankAccountRequest(string Name, string Description);
public record struct CreateBankAccountReponse(string Id, string Name);

public class CreateBankAccountEndpoint : IEndpoint
{
    private const string Route = "bank-accounts";
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(Route, CreateBankAccountAsync)
           .WithTags("bank-account")
           //.RequireAuthorization()
           .WithName("Create a new bank account");
    }

    private async static Task<IResult> CreateBankAccountAsync(
     CreateBankAccountRequest command,
     CancellationToken cancellationToken)
    {
        // Simula a criação da conta
        var account = new
        {
            Id = Guid.NewGuid().ToString(),
            Name = command.Name
        };
        await Task.CompletedTask;
        return Results.Created(
           $"{Route}/{account.Id}",
           new CreateBankAccountReponse(account.Id, account.Name));
    }
}


//https://localhost:7151/api/v1/bank-accounts

/*
 * {
    "id": "8f981106-2fe7-4f8b-8157-eb49a9b9d8bf",
    "name": "Farley"
}
 */