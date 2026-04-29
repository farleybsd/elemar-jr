using PicPayManager.Crosscutting.ValueObjects.Transference;
using PicPayManager.UserAccounts;

namespace PicPayManager.Transference;

public class Transaction
{
    private Transaction() { }
    
        
    
    public Transaction(TransactionOrigin transactionOrigin, TransactionDestination transactionDestination, FormasPagamento tipoPagamento, TransferBalance transferBalance)
    {
        TransactionOrigin = transactionOrigin;
        TransactionDestination = transactionDestination;
        TipoPagamento = tipoPagamento;
        TransferBalance = transferBalance;
    }

    public int Id { get; set; }
    public TransactionOrigin TransactionOrigin { get; }
    public TransactionDestination TransactionDestination { get; set; }
    public FormasPagamento TipoPagamento { get; init; } = FormasPagamento.Pix;
    public TransferBalance TransferBalance { get; }
}
