namespace SINGLERESPONSIBILITYPRINCIPLE_SRP_.ResultPatern
{
    public interface IResult
    {
        string Error { get; }
        bool IsFailure { get; }
        bool IsSuccess { get; }

     
    }
}