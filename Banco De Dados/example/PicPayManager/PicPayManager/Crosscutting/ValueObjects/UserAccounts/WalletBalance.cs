namespace PicPayManager.Crosscutting.ValueObjects.UserAccounts;

public struct WalletBalance
{
    public decimal Amount { get; private set; }

    public WalletBalance(decimal amount) => Amount = amount;

    public static WalletBalance Parse(decimal amount)
        => (TryParse(amount, out WalletBalance result))
        ? result
        : throw new ArgumentOutOfRangeException(nameof(amount), "O saldo não pode ser negativo.");

    public static bool TryParse(decimal amount, out WalletBalance result)
    {
        if (!IsValid(amount))
        {
            result = default;
            return false;
        }
        result = new WalletBalance(amount);
        return true;
    }

    public static bool IsValid(decimal amount)
    {
        if (amount < 0)
            return false;
        return true;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "O valor do débito deve ser maior que zero.");

        if (amount > Amount)
            throw new InvalidOperationException("Saldo insuficiente.");

        Amount -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "O valor do crédito deve ser maior que zero.");
        Amount += amount;
    }
}