namespace Toolbox.Pages.Models;

public class ApiResult
{
    public int Code { get; set; }
    public string Message { get; set; } = "";
}

public class ApiResult<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }
}