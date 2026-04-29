using Microsoft.Identity.Client;

namespace PicPayManager.Crosscutting.ValueObjects.Transference;

public struct TransactionOrigin
{
    public string _value { get;}
    public TransactionOrigin(string value) => _value = value;

    public static implicit operator TransactionOrigin(string value) => Parse(value);

    public static TransactionOrigin Parse(string value)
        => (TryParse(value, out TransactionOrigin result))
        ? result
        : throw new ArgumentException($"Invalid value for TransactionOrigin: {value}");

    public static bool TryParse(string value,out TransactionOrigin result)
    {
        if(string.IsNullOrEmpty(value) || !IsValid(value))
        {
            result = default;
            return false;
        }

        result = new TransactionOrigin(value);
        return true;
    }

    public override string ToString() => _value;
    public bool IsEmpty() => string.IsNullOrEmpty(_value);
    public static bool IsValid(string value) {  /*Validacao Qualquer*/ return true; }
}
