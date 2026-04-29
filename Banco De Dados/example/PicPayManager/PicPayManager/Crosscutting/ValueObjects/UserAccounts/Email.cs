namespace PicPayManager.Crosscutting.ValueObjects.UserAccounts;

public struct Email
{
    public string _value { get; }

    public Email(string value) => _value = value;

    public static implicit operator Email(string value) => Parse(value);

    public static Email Parse(string value)
        => (TryParse(value, out var result))
        ? result
        : throw new ArgumentException(nameof(value));

    public static bool TryParse(string value, out Email result)
    {
        if (string.IsNullOrEmpty(value) || !IsValid(value))
        {
            result = default;
            return false;
        }
        else
        {
            result = new Email(value);
            return true;
        }
    }

    public override string ToString() => _value;

    public bool IsEmpity => string.IsNullOrEmpty(_value);

    public static bool IsValid(string value)
    {/*Email Validatiom*/ return true; }
}