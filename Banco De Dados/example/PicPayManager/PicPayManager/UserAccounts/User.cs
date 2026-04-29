using PicPayManager.Crosscutting.Enum;
using PicPayManager.Crosscutting.ValueObjects.UserAccounts;
using PicPayManager.Transference;


namespace PicPayManager.UserAccounts;

public class User
{
    private readonly List<Transaction> _transactions = new();

    private User() { }
    public User(Name name, Cpf cpf, Email email, WalletBalance walletBalance)
    {
        Name = name;
        Cpf = cpf;
        Email = email;
        WalletBalance = walletBalance;
    }

    public int Id { get; set; }
    public Name Name { get; }
    public Cpf Cpf { get; }
    public Email Email { get; }
    public WalletBalance WalletBalance { get; }
    public UserType UserType { get; init; } = UserType.Commonuser;
    public IReadOnlyCollection<Transaction> Transactions => _transactions;
    public bool HasBalance => WalletBalance.Amount > 0;
    public void Debit(decimal amount) => WalletBalance.Debit(amount);

    public void Credit(decimal amount) => WalletBalance.Credit(amount);
}