namespace Composicao;

public  struct StudentDateOfBirth
{
    private readonly DateOnly _value;

    public StudentDateOfBirth(DateOnly value) => _value = value;
    public static StudentDateOfBirth Parse(DateOnly value)
        => (TryParse(value, out var result) 
        ? result 
        : throw new ArgumentException("Date of birth cannot be in the future.", nameof(value)));
    

    public static bool TryParse(DateOnly value,out StudentDateOfBirth result)
    {
        if (value > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(value));
            result = default;
            return false;
        }

        result = new StudentDateOfBirth(value);
        return true;

    }

    public static implicit operator StudentDateOfBirth(DateOnly value) => Parse(value) ;
    public override string ToString() => _value.ToString("yyyy-MM-dd");
    public bool IsEmpty() => _value == default;
    public static bool IsValid(DateOnly value) {/**/ return true; }

}


