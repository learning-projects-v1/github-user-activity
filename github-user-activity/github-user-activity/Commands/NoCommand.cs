using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace github_user_activity.Commands
{
    public class NoCommand : ICommand
    {
        public async Task ExecuteAsync()
        {
            Console.WriteLine("Invalid Command name");
        }
    }
}
