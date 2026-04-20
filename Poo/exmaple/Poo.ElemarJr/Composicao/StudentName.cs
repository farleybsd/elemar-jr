namespace Composicao;
public struct StudentName
{
    private readonly string _value;
    public StudentName(string value) => _value = value;

    public static StudentName Parse(string value) => (TryParse(value, out var result)) ? result : throw new ArgumentException(nameof(value));

    public static bool TryParse(string value,out StudentName result)
    {
        if(string.IsNullOrEmpty(value) || !IsValid(value))
        {
            result = default;
            return false;
        }
        result = new StudentName(value);
        return true;
    }

    public static implicit operator StudentName(string value) => Parse(value);
    public override string ToString() => _value;
    public bool IsEmpty => string.IsNullOrEmpty(_value);
    public static bool IsValid(string value) { /*Validacao*/ return true; }
}