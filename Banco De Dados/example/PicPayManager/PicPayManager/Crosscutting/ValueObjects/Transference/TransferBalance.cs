namespace PicPayManager.Crosscutting.ValueObjects.Transference;

public struct TransferBalance
{
    public decimal _balance { get; }
    public TransferBalance(decimal balance) => _balance = balance;

    public static bool TryParse(decimal balance,out TransferBalance result)
    {
        if(Isvalid(balance))
        {
            result = new TransferBalance(balance);
            return true;
        }
        result = default;
        return false;
    }

    public static TransferBalance Parse(decimal balance)
        => (TryParse(balance, out var result)) 
        ? result 
        : throw new ArgumentException("Invalid balance value.");

    public static implicit operator TransferBalance(decimal value) => Parse(value);
    public bool IsEmpty() => _balance == 0;

    public static bool Isvalid(decimal balance)
    {
        return balance > 0;
    }

}
