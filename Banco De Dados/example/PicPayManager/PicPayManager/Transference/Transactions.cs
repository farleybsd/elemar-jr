using PicPayManager.Crosscutting.ValueObjects.Transference;
using PicPayManager.UserAccounts;

namespace PicPayManager.Transference;

public class Transactions
{
    private Transactions() { }
    
        
    
    public Transactions(TransactionOrigin transactionOrigin, TransactionDestination transactionDestination, TransferBalance transferBalance, int userId)
    {
        TransactionOrigin = transactionOrigin;
        TransactionDestination = transactionDestination;
        TransferBalance = transferBalance;
        UserId = userId;
    }

    public int Id { get;  }
    public int UserId { get; }
    public TransactionOrigin TransactionOrigin { get; }
    public TransactionDestination TransactionDestination { get; set; }
    public FormasPagamento TipoPagamento { get; init; } = FormasPagamento.Pix;
    public TransferBalance TransferBalance { get; }
}
