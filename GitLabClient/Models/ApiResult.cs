namespace GitLab.Models;

using GitLab.Models.Error;

public class ApiResult<T> where T : GitLabModel
{
    private T? _result;
    private GitLabError? _error;
    public System.Boolean Ok => _result != null && _error == null;
    public T Result => _result ?? throw new NotImplementedException("do not grab Result when it's not Ok");
    public GitLabError Error => _error;
    private ApiResult(T result)
    {
        _result = result;
    }
    private ApiResult(GitLabError error)
    {
        _error = error;
    }
    public static ApiResult<T> MakeSuccess(T result)
    {
        return new ApiResult<T>(result);
    }
    public ApiResult<T> WithClient(GitLabClient client)
    {
        Result.WithClient(client);
        return this;
    }
    public static ApiResult<T> MakeError(System.String error)
    {
        return new ApiResult<T>(
            new GitLabError
            {
                Message = error
            }
        );
    }
}