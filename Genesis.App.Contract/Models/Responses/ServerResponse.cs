namespace Genesis.App.Contract.Models.Responses
{
    public record ServerResponse(ResultCode code, string message = null);
    public record ServerResponse<T>(ResultCode code, T data, string message = null) : ServerResponse(code, message);
}