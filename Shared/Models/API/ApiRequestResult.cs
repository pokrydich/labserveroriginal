namespace LabServer.Shared.Models;

using GitLab.Models;

public class ApiRequestResult
{
    public System.Boolean Successful { get; set; }
    public HashSet<System.String> Warnings { get; set; } = new HashSet<string>();
    public System.String? Error { get; set; }
    public static ApiRequestResult Success(HashSet<System.String>? warnings = null)
    {
        return new ApiRequestResult 
        { 
            Successful = true, 
            Warnings = warnings ?? new HashSet<string>()
        };
    }
    public static ApiRequestResult<T> Success<T>(T result, HashSet<System.String>? warnings = null)
    {
        return new ApiRequestResult<T>
        {
            Successful = true,
            Result = result,
            Warnings = warnings ?? new HashSet<string>()
    };
    }
    public static ApiRequestResult Failure(System.String message)
    {
        return new ApiRequestResult 
        { 
            Successful = false, 
            Error = message, 
            Warnings = new HashSet<string>() 
        };
    }
    public static ApiRequestResult<T> Failure<T>(System.String message)
    {
        return new ApiRequestResult<T> 
        { 
            Successful = false, 
            Result = default(T), 
            Error = message, 
            Warnings = new HashSet<string>() 
        };
    }
}
public class ApiRequestResult<T> : ApiRequestResult
{
    public T? Result {get;set;}
}