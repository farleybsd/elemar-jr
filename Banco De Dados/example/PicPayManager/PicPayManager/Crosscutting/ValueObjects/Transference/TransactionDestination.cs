namespace PicPayManager.Crosscutting.ValueObjects.Transference;

public struct TransactionDestination
{
    public string _value { get; }
    public TransactionDestination(string value) => _value = value;

    public static implicit operator TransactionDestination(string value) => Parse(value);

    public static TransactionDestination Parse(string value)
        => (TryParse(value, out TransactionDestination result))
        ? result
        : throw new ArgumentException($"Invalid value for TransactionDestination: {value}");

    public static bool TryParse(string value, out TransactionDestination result)
    {
        if (string.IsNullOrEmpty(value) || !IsValid(value))
        {
            result = default;
            return false;
        }

        result = new TransactionDestination(value);
        return true;
    }

    public override string ToString() => _value;
    public bool IsEmpty() => string.IsNullOrEmpty(_value);
    public static bool IsValid(string value) {  /*Validacao Qualquer*/ return true; }
}
