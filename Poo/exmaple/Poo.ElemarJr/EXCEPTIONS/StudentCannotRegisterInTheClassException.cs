using System.Diagnostics.CodeAnalysis;

namespace EXCEPTIONS;

public class StudentCannotRegisterInTheClassException : Exception
{
    public string? StudentName { get; }
    public string? ClassCode { get; }
    public StudentCannotRegisterInTheClassException() { }
    public StudentCannotRegisterInTheClassException(string message)
    : base(message) { }
    public StudentCannotRegisterInTheClassException(string message, Exception innerException)
    : base(message, innerException) { }
    public StudentCannotRegisterInTheClassException(string message, string studentName, string classCode)
    : this(message)
    {
        StudentName = studentName;
        ClassCode = classCode;
    }
    [DoesNotReturn]
    public static void ThrowIf(bool condition, string message, string studentName, string classCode)
    {
        if (condition)
            throw new StudentCannotRegisterInTheClassException(message, studentName, classCode);
    }
}
