using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace github_user_activity.Commands
{
    public class ShowForkedRepoCommand
    {
        private readonly GithubApiService _githubApiService;
        private string _input;
        public ShowForkedRepoCommand(GithubApiService githubApiService, string input)
        {
            _githubApiService = githubApiService;
            _input = input;
        }
        public async Task ExecuteAsync()
        {
            //await _githubApiService.ShowForkedRepo(_input);

        }
    }
}
