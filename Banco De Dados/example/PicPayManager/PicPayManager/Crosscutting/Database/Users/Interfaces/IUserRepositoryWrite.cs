using PicPayManager.UserAccounts;

namespace PicPayManager.Crosscutting.Database.Users.Interfaces;

public interface IUserRepositoryWrite
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(User user, CancellationToken cancellationToken);
}