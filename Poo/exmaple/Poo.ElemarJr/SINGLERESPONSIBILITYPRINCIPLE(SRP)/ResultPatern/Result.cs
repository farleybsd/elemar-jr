using System;
using System.Collections.Generic;
using System.Text;

namespace SINGLERESPONSIBILITYPRINCIPLE_SRP_.ResultPatern;


public readonly struct Result<T> : IResult
{
    private Result(string error, bool isFailure, T value)
    {
        Error = error;
        IsFailure = isFailure;
        _value = value;
    }

    private readonly T? _value;

    public string Error { get; }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on failure.");

    public static Result<T> Failure(string error)
        => new Result<T>(error, true, default!);

    public static implicit operator Result<T>(T value)
        => new Result<T>(string.Empty, false, value);
}