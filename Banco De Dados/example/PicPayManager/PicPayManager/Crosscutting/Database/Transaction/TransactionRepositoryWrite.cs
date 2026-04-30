using PicPayManager.Transference;

namespace PicPayManager.Crosscutting.Database.Transaction;

public interface ITransactionRepositoryWrite
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(Transactions transaction, CancellationToken cancellationToken);
}
public class TransactionRepositoryWrite : ITransactionRepositoryWrite
{
    private readonly PicPaySimplificadoContext _context;

    public TransactionRepositoryWrite(PicPaySimplificadoContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task AddAsync(Transactions transaction, CancellationToken cancellationToken)
    {
        await _context
                .Transactions
                .AddAsync(transaction, cancellationToken)
                .ConfigureAwait(false);
    }
}