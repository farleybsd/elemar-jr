namespace Api.IntelligentServiceFindZipCode.Crosscutting;

public static class ValidationErrors
{
    public static class General
    {
        public static ErrorMessage UnknownError(string message) => new("UNKNOWN_ERROR", message);
    }
}
