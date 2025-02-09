using github_user_activity;
using System.Composition;
using System.Composition.Hosting;

class Program
{
    // The Main method is now async
    public static async Task Main(string[] args)
    {
        var username = "Zakir-sust";
        Console.WriteLine("List of available commands: \n1.show-repo\n2.show-lang\n3.#(for exit)");
        while (true)
        {
            Console.WriteLine("Enter a command:");
            string input = Console.ReadLine();
            if (input == "#") break;
            var factory = new CommandFactory();
            var command = factory.GetCommand(input + " " + username);
            await command.ExecuteAsync();
            Console.WriteLine("------------------------");
        }

        // Execute the command asynchronously
    }
}


[Export]
public class MyApplication
{
    public async Task RunAsync()
    {
        var username = "Zakir-sust";
        while (true)
        {
            string input = Console.ReadLine();
            if (input == "#") break;
            var factory = new CommandFactory();
            var command = factory.GetCommand(input + " " + username);
            await command.ExecuteAsync();
        }
    }
}
