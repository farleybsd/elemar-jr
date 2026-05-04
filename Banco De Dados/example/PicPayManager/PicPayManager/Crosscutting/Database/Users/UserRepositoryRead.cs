using MaybeMonad;
using Microsoft.EntityFrameworkCore;
using PicPayManager.Crosscutting.Database.Users.Interfaces;
using PicPayManager.UserAccounts;
using PicPayManager.UserAccounts.Endpoints;

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
                             .TagWith("Buscar Todos Os Usuarios!")
                             .TagWithCallSite()
                             .AsNoTracking()
                             .ToListAsync(cancellationToken);
    }

    public async Task<Maybe<IReadOnlyList<SearchAllUserTransactionsResponse>>> GetTransactionUserByIdAsync(int id, int skip = 0, int take = 10, CancellationToken cancellationToken = default)
    {
        var transactionUsers = await _context
           .Users
           .Include(ac => ac.Transactions)
           .Skip(skip)
           .Take(take)
           .Where(ac => ac.Id == id)
           .Select(bc => new SearchAllUserTransactionsResponse(
               name: bc.Name._value,
               Cpf: bc.Cpf._value,
               Email: bc.Email._value,
               walletBalance: bc.WalletBalance.Amount
           ))
           .ToListAsync(cancellationToken: cancellationToken);

        if( transactionUsers is null)
            return Maybe<IReadOnlyList<SearchAllUserTransactionsResponse>>.Nothing;

        return transactionUsers;
    }
}