using Microsoft.EntityFrameworkCore;
using PicPayManager.Crosscutting.Database;
using PicPayManager.Crosscutting.Database.Users.Interfaces;
using PicPayManager.Crosscutting.Interfaces;
using PicPayManager.Crosscutting.ValueObjects.UserAccounts;
using System.ComponentModel.DataAnnotations;

namespace PicPayManager.UserAccounts.Endpoints;

public record struct CreateUserAccountRequest(string name, string Cpf, string Email, decimal walletBalance);
public record struct CreateUserAccountReponse(int Id, string Name);

public class CreateBankAccount : IEndpoint
{
    private const string Route = "user-accounts";

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(Route, CreateUserAccountAsyncHandler)
           .WithTags("user-account")
           .WithName("Create a new user account");
    }

    //private static async Task<IResult> CreateUserAccountAsyncHandler(
    //  IUserRepositoryWrite _userRepositoryWrite,
    //  CreateUserAccountRequest request,
    // CancellationToken cancellationToken)
    //{
    //    var context = (_userRepositoryWrite.UnitOfWork as PicPaySimplificadoContext)
    //    ?? throw new InvalidOperationException("Contexto inválido");

    //    await using var transaction = await context.Database
    //    .BeginTransactionAsync(cancellationToken);

    //    var name = Name.Parse(request.name);
    //    var cpf = Cpf.Parse(request.Cpf);
    //    var email = Email.Parse(request.Email);
    //    var walletBalance = WalletBalance.Parse(request.walletBalance);
    //    var user = new User(name, cpf, email, walletBalance);
    //    await _userRepositoryWrite.AddAsync(user, cancellationToken);

    //    var result = await _userRepositoryWrite.UnitOfWork.SaveEntitiesAsync(cancellationToken).ConfigureAwait(false);

    //    if (result)
    //    {
    //        await transaction.CommitAsync(cancellationToken);
    //    }
    //    else
    //    {
    //        await transaction.RollbackAsync(cancellationToken);
    //        return Results.Problem("Ocorreu um erro ao criar a conta do usuário.");
    //    }
    //    await Task.CompletedTask;
    //    return Results.Created(
    //       $"{Route}/{user.Id}",
    //       new CreateUserAccountReponse(user.Id, user.Name.ToString()));
    //}

    private static async Task<IResult> CreateUserAccountAsyncHandler(
    IUserRepositoryWrite userRepositoryWrite,
    CreateUserAccountRequest request,
    CancellationToken cancellationToken)
    {
        var context = (userRepositoryWrite.UnitOfWork as PicPaySimplificadoContext)
            ?? throw new InvalidOperationException("Contexto inválido");

        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var name = Name.Parse(request.name);
                var cpf = Cpf.Parse(request.Cpf);
                var email = Email.Parse(request.Email);
                var walletBalance = WalletBalance.Parse(request.walletBalance);

                var user = new User(name, cpf, email, walletBalance);

                await userRepositoryWrite.AddAsync(user, cancellationToken);

                var result = await userRepositoryWrite.UnitOfWork
                    .SaveEntitiesAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (!result)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return Results.Problem(
                        "Ocorreu um erro ao criar a conta do usuário.");
                }

                await transaction.CommitAsync(cancellationToken);

                return Results.Created(
                    $"{Route}/{user.Id}",
                    new CreateUserAccountReponse(
                        user.Id,
                        user.Name.ToString()));
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}