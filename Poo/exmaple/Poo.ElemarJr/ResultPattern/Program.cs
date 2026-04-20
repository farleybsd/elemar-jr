
public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    string Error { get; }
}
public readonly struct Result : IResult
{
    private Result(string error, bool isFailure)
    {
        Error = error;
        IsFailure = isFailure;
    }

    public string Error { get; }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;

    public static Result Failure(string error)
        => new Result(error, true);

    public static Result Success()
        => new Result(string.Empty, false);
}
public readonly struct Result<T> : IResult
{
    private Result(string error, bool isFailure, T value)
    {
        Error = error;
        IsFailure = isFailure;
        Value = value;
    }
    public string Error { get; }
    public bool IsFailure { get; }
    public T Value { get; }
    public bool IsSuccess => !IsFailure;
    public static Result<T> Failure(string error)
    => new Result<T>(error, true, default!);
    public static implicit operator Result<T>(T value)
    => new Result<T>(string.Empty, false, value);
    public static implicit operator Result<T>(Result result)
    => new Result<T>(result.Error, true, default!);
}

    public override bool CanRegister(Student student)
=> IsOpen() && student.Age <= AgeLimit;

    public override Result CanRegister(Student student)
    {
        if (!IsOpen())
            return Result.Failure("Turma não está aberta.");
        if (student.Age > AgeLimit)
            return Result.Failure("Idade superior ao permitido para a turma.");
        return Result.Success();
    }

public interface IResult2
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    string Error { get; }
}

public readonly struct Result2 : IResult2
{
    public Result2(string error, bool isFailure)
    {
        Error = error;
        IsFailure = isFailure;
    }

    public string Error { get; }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;

    public static Result2 Success()
        => new Result2(string.Empty, false);

    public static Result2 Failure(string error)
        => new Result2(error, true);
}