namespace Imanys.SolenLms.Application.Shared.Core.UseCases;

public class RequestResponse
{
    private ResponseError? _responseError;

    public bool IsSuccess { get; init; }
    public string? Message { get; init; }

    public ResponseError? GetResponseError() => _responseError;

    public void SetResponseStatus(ResponseError status)
    {
        _responseError = status;
    }

    public static RequestResponse Ok(string? message = null) => new() { IsSuccess = true, Message = message };

    public static RequestResponse Error(ResponseError responseError, string? message = null)
    {
        RequestResponse response = new() { IsSuccess = false, Message = message };
        response.SetResponseStatus(responseError);
        return response;
    }

    public static RequestResponse Error(string? message = null)
    {
        RequestResponse response = new() { IsSuccess = false, Message = message };
        response.SetResponseStatus(ResponseError.Unprocessable);
        return response;
    }
}

public sealed class RequestResponse<T> : RequestResponse
{
    public T? Data { get; init; }

    public static RequestResponse<T> Ok(string? message = null, T? data = default) =>
        new() { IsSuccess = true, Message = message, Data = data };

    public static new RequestResponse<T> Error(ResponseError responseError, string? message = null)
    {
        RequestResponse<T> response = new() { IsSuccess = false, Message = message };
        response.SetResponseStatus(responseError);
        return response;
    }

    public static new RequestResponse<T> Error(string? message = null)
    {
        RequestResponse<T> response = new() { IsSuccess = false, Message = message };
        response.SetResponseStatus(ResponseError.Unprocessable);
        return response;
    }

    public static RequestResponse<T> NotFound(string? message = null)
    {
        RequestResponse<T> response = new() { IsSuccess = false, Message = message };
        response.SetResponseStatus(ResponseError.NotFound);
        return response;
    }
}