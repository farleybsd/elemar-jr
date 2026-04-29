namespace PicPayManager.Crosscutting.ValueObjects.UserAccounts;

public struct Cpf
{
    public string _value { get; }

    public Cpf(string value) => _value = value;

    public static Cpf Parse(string value)
        => (TryParse(value, out var result))
        ? result
        : throw new ArgumentException(nameof(value));

    public static bool TryParse(string value, out Cpf result)
    {
        if (string.IsNullOrEmpty(value) || !IsValid(value))
        {
            result = default;
            return false;
        }
        else
        {
            result = new Cpf(value);
            return true;
        }
    }

    public static implicit operator Cpf(string value) => Parse(value);

    public override string ToString() => _value;

    public bool IsEmpty => string.IsNullOrEmpty(_value);

    public static bool IsValid(string value)
    { /* CPF validation */  return true; }
}