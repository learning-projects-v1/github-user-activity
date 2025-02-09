
namespace github_user_activity.Commands;

//show forked by taking filters
public class ShowAllRepoCommand : ICommand
{
    private readonly GithubApiService _githubApiService;
    private string _input;
    public ShowAllRepoCommand(GithubApiService githubApiService, string input)
    {
        _githubApiService = githubApiService;
        _input = input;
    }
    public async Task ExecuteAsync()
    {
        await _githubApiService.ShowAllRepo(_input);
    }
}