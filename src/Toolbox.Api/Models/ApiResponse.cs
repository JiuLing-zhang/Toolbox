using System.Text.Json.Serialization;

namespace Toolbox.Api.Models;
public class ApiResponse
{
    public int Code { get; set; }
    public string Message { get; set; }

    public ApiResponse(int code, string message)
    {
        Code = code;
        Message = message;
    }
}

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    public ApiResponse(int code, string message, T? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }
}