using Microsoft.EntityFrameworkCore;
using PicPayManager.Crosscutting.Database.Users.Interfaces;
using PicPayManager.UserAccounts;

namespace PicPayManager.Crosscutting.Database.Users;

public class UserRepositoryRead : IUserRepositoryRead
{
    private readonly PicPaySimplificadoContext _context;

    public UserRepositoryRead(PicPaySimplificadoContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Users
        .AsNoTracking()
        .ToListAsync(cancellationToken);
    }
}