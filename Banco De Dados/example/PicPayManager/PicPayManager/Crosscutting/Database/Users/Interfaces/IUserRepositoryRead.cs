using MaybeMonad;
using PicPayManager.UserAccounts;
using PicPayManager.UserAccounts.Endpoints;

namespace PicPayManager.Crosscutting.Database.Users.Interfaces;

public interface IUserRepositoryRead
{
    IUnitOfWork UnitOfWork { get; }

    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken);
    Task<Maybe<IReadOnlyList<SearchAllUserTransactionsResponse>>> GetTransactionUserByIdAsync(int id, int skip = 0, int take = 10, CancellationToken cancellationToken = default);
}