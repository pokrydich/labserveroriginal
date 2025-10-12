namespace GitLab.Models.Error;

// TODO: deserialize error (can be str or dictrionary with lists of strings)
public class GitLabError
{
    public System.String Message { get; set; } = System.String.Empty;
}