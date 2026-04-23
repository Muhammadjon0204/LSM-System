namespace Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; private set; }    

    public static Result<T> Success(T data, string message = "") => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message,
        StatusCode = 200
    };

    public static Result<T> Failure(string message, int statusCode = 400) => new()
    {
        IsSuccess = false,
        Message = message,
        StatusCode = statusCode
    };

    public static Result<T> NotFound(string message) => Failure(message, 404);
    public static Result<T> Forbidden(string message) => Failure(message, 403);
    public static Result<T> Unknown(string message) => Failure(message, 404);
    public static Result<T> Unauthorized(string message) => Failure(message, 401);
}