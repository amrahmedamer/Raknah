namespace Raknah.Abstractions;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new Result(true, Error.None);
    public static Result Failure(Error error) => new Result(false, error);
    public static Result<T> Success<T>(T value) => new Result<T>(true, Error.None, value);
    public static Result<T> Failure<T>(Error error) => new Result<T>(false, error, default!);
}

public class Result<T>(bool isSuccess, Error error, T value) : Result(isSuccess, error)
{

    public T Value { get; } = isSuccess
         ? value!
         : default!;
}
