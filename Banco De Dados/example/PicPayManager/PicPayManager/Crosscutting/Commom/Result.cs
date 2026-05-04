namespace PicPayManager.Crosscutting.Commom;

public interface IOperationResult
{
    string Error { get; }
    bool IsFailure { get; }
    bool IsSuccess { get; }
}
public readonly struct Result : IOperationResult
{
    public Result(string error, bool isFailure)
    {
        Error = error;
        IsFailure = isFailure;
    }
    public string Error { get; }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;
    public static Result Success()
    => new Result(string.Empty, false);
    public static Result Failure(string error)
    => new Result(error, true);
}
