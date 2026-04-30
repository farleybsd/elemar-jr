using Microsoft.EntityFrameworkCore;
using PicPayManager.Crosscutting.Database;
using PicPayManager.Crosscutting.Database.Transaction;
using PicPayManager.Crosscutting.Database.Users.Interfaces;
using PicPayManager.Crosscutting.Interfaces;
using PicPayManager.Crosscutting.ValueObjects.Transference;
using PicPayManager.Crosscutting.ValueObjects.UserAccounts;
using PicPayManager.Transference;
using System.ComponentModel.DataAnnotations;

namespace PicPayManager.UserAccounts.Endpoints;

public record struct CreateTransactionUserRequest(string TransactionOrigin, string TransactionDestination, decimal TransferBalance, int userId);
public record struct CreateTransactionUserReponse(int Id, string TransactionDestination, string TransactionOrigin, decimal TransferBalance, FormasPagamento tipoPagamento);

public class CreateTransactionUser : IEndpoint
{
    private const string Route = "user-transaction";

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(Route, CreateUserAccountAsyncHandler)
           .WithTags("user-transaction")
           .WithName("Create a new user transaction");
    }

   

    private static async Task<IResult> CreateUserAccountAsyncHandler(
    ITransactionRepositoryWrite transactionRepositoryWrite,
    CreateTransactionUserRequest request,
    CancellationToken cancellationToken)
    {
        var context = (transactionRepositoryWrite.UnitOfWork as PicPaySimplificadoContext)
            ?? throw new InvalidOperationException("Contexto inválido");

        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                
                var transactionOrigin = TransactionOrigin.Parse(request.TransactionOrigin);
                var transactionDestination = TransactionDestination.Parse(request.TransactionDestination);
                var transferBalance = TransferBalance.Parse(request.TransferBalance);

                var TransactionUser = new Transactions(transactionOrigin, transactionDestination, transferBalance, request.userId);

                await transactionRepositoryWrite.AddAsync(TransactionUser, cancellationToken);

                var result = await transactionRepositoryWrite.UnitOfWork
                    .SaveEntitiesAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (!result)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return Results.Problem(
                        "Ocorreu um erro na transferencia para a conta do usuário.");
                }

                await transaction.CommitAsync(cancellationToken);

                return Results.Created(
                    $"{Route}/{TransactionUser.Id}",
                    new CreateTransactionUserReponse(
                        TransactionUser.Id,
                        TransactionUser.TransactionDestination.ToString(),
                        TransactionUser.TransactionOrigin.ToString(),
                        TransactionUser.TransferBalance._balance,
                        TransactionUser.TipoPagamento
                    ));
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}