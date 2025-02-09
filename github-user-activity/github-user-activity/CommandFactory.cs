using github_user_activity.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace github_user_activity;

public class CommandFactory
{
    public GithubApiService _githubApiService { get; set; }
    public CommandFactory()
    {
        _githubApiService = new GithubApiService();
    }
    public ICommand GetCommand(string input)
    {
        var inputs = input.Split(' ');
        
        switch (inputs[0])
        {
            case "show-repo":
                return new ShowAllRepoCommand(_githubApiService, inputs[1]);
            case "show-lang":
                return new ShowLanguagesCommand(_githubApiService, inputs[1]);
            default:
                return new NoCommand();
        }
    }
}
