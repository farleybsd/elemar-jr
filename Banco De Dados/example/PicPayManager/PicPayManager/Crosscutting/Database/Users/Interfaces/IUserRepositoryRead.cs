using PicPayManager.UserAccounts;

namespace PicPayManager.Crosscutting.Database.Users.Interfaces;

public interface IUserRepositoryRead
{
    IUnitOfWork UnitOfWork { get; }

    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken);
}