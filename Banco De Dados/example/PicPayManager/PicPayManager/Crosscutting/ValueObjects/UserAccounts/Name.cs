using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PicPayManager.Crosscutting.ValueObjects.UserAccounts;

public struct Name
{
    public string _value { get; }

    public Name(string value) => _value = value;

    public static implicit operator Name(string value) => Parse(value);

    public static Name Parse(string value)
        => (TryParse(value, out var result))
        ? result
         : throw new ArgumentException(nameof(value));

    public static bool TryParse(string value, out Name result)
    {
        if (string.IsNullOrEmpty(value) || !IsValid(value))
        {
            result = default;
            return false;
        }
        result = new Name(value);
        return true;
    }

    public override string ToString() => _value;

    public bool IsEmpty => string.IsNullOrEmpty(_value);

    public static bool IsValid(string valie)
    { /* Validacao*/ return true; }
}