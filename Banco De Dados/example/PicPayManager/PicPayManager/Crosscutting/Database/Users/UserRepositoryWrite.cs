using PicPayManager.Crosscutting.Database.Users.Interfaces;
using PicPayManager.UserAccounts;

namespace PicPayManager.Crosscutting.Database.Users;

public class UserRepositoryWrite : IUserRepositoryWrite
{
    private readonly PicPaySimplificadoContext _context;

    public UserRepositoryWrite(PicPaySimplificadoContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context
                .Users
                .AddAsync(user, cancellationToken)
                .ConfigureAwait(false);
    }
}