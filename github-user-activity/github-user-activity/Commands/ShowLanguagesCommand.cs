using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace github_user_activity.Commands;

public class ShowLanguagesCommand : ICommand
{
    private readonly GithubApiService _githubApiService;
    private string _input;
    public ShowLanguagesCommand(GithubApiService githubApiService, string input)
    {
        _githubApiService = githubApiService;
        _input = input;
    }
    public async Task ExecuteAsync()
    {
        Console.WriteLine("Executing Command : ShowLangugages");
        await _githubApiService.ShowAllRepoLanguages(_input);
    }
}
