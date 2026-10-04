namespace EBI.ALAS.Api.Shared.Models;

public readonly record struct Error(string Code, string Message)
{
    public static Error None => new(string.Empty, string.Empty);
    public bool IsNone => string.IsNullOrEmpty(Code);
}

public readonly record struct Result
{
    private readonly Error _error;

    private Result(Error error)
    {
        _error = error;
    }

    public bool IsSuccess => _error.IsNone;
    public bool IsFailure => !IsSuccess;
    public Error Error => _error;

    public static Result Success() => new(Error.None);
    public static Result Failure(Error error) => new(error);
    public static Result Failure(string code, string message) => new(new Error(code, message));
}

public readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly Error _error;

    private Result(T? value, Error error)
    {
        _value = value;
        _error = error;
    }

    public bool IsSuccess => _error.IsNone;
    public bool IsFailure => !IsSuccess;
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value on failed result");
    public Error Error => _error;

    public static Result<T> Success(T value) => new(value, Error.None);
    public static Result<T> Failure(Error error) => new(default, error);
    public static Result<T> Failure(string code, string message) => new(default, new Error(code, message));

    public Result<TNew> Map<TNew>(Func<T, TNew> mapper) =>
        IsSuccess ? Result<TNew>.Success(mapper(Value)) : Result<TNew>.Failure(Error);

    public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper) =>
        IsSuccess ? Result<TNew>.Success(await mapper(Value)) : Result<TNew>.Failure(Error);
}