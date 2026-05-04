using Microsoft.AspNetCore.Mvc;
using PicPayManager.Crosscutting.Database.Users.Interfaces;
using PicPayManager.Crosscutting.Interfaces;

namespace PicPayManager.UserAccounts.Endpoints;

public readonly record struct SearchAllUserTransactionsRequest(int id, int skip = 0, int take = 10);
public readonly record struct SearchAllUserTransactionsResponse(string name, string Cpf, string Email, decimal walletBalance);
public class SearchAllUserTransactions : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("SearchAll-User-Transactions", GetBankAccountsAsync)
            .WithTags("SearchAll-User-Transactions")
            .WithName("All user transactions found.");
    }


    private async static Task<IResult> GetBankAccountsAsync(
        IUserRepositoryRead repository,
        ILogger<SearchAllUserTransactions> logger,
        [AsParameters] SearchAllUserTransactionsRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all user transactions");

        var transactionUsers = await repository.GetTransactionUserByIdAsync(request.id, request.skip, request.take, cancellationToken);

        return Results.Ok(transactionUsers);
    }
}

