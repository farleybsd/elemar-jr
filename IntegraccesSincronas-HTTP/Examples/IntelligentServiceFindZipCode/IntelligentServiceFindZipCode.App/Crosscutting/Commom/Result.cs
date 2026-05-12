namespace IntelligentServiceFindZipCode.App.Crosscutting.Commom;
public interface IOperationResult
{
    string Error { get; }
    bool IsFailure { get; }
    bool IsSuccess { get; }
}
public readonly struct Result<T> : IOperationResult
{
    private Result(T? value, string error, bool isFailure)
    {
        Value = value;
        Error = error;
        IsFailure = isFailure;
    }

    public T? Value { get; }

    public string Error { get; }

    public bool IsFailure { get; }

    public bool IsSuccess => !IsFailure;

    public static Result<T> Success(T value)
        => new(value, string.Empty, false);

    public static Result<T> Failure(string error)
        => new(default, error, true);
}

