namespace Urll.TelegramBot;

public class CommandsExecutor
{
    public async Task<string> Execute(string input)
    {
        string[] args = input.Split(' ');
        if (args.Length == 0)
        {
            return GetHelpText();
        }

        if (!args[0].StartsWith('/'))
        {
            return await ExecuteAddCommand(args);
        }

        string command = args[0];
        args = args[1..];
        return command switch
        {
            "/start" => ExecuteStartCommand(),
            "/list" => await ExecuteListCommand(args),
            "/get" => await ExecuteGetCommand(args),
            "/delete" => await ExecuteDeleteCommand(args),
            _ => "Command not found",
        };
    }

    private string GetHelpText()
    {
        return @"Commands
{url} {code} - Add Link
/start - Show help
/list - List all Links
/get {code} - Return Link by code
/delete {code} - Delete Link";
    }

    private string ExecuteStartCommand()
    {
        return GetHelpText();
    }

    private async Task<string> ExecuteListCommand(string[] args)
    {
        if (args.Length != 0)
        {
            return GetHelpText();
        }

        await Task.Delay(10);
        return "List all Links";
    }

    private async Task<string> ExecuteGetCommand(string[] args)
    {
        if (args.Length != 1)
        {
            return GetHelpText();
        }

        await Task.Delay(10);
        return $"Get Link with code '{args[0]}'";
    }

    private async Task<string> ExecuteDeleteCommand(string[] args)
    {
        if (args.Length != 1)
        {
            return GetHelpText();
        }

        await Task.Delay(10);
        return $"Delete Link with code '{args[0]}'";
    }

    private async Task<string> ExecuteAddCommand(string[] args)
    {
        if (args.Length != 2)
        {
            return GetHelpText();
        }

        await Task.Delay(10);
        return $"Add Link\nUrl: '{args[0]}'\nCode: '{args[1]}'";
    }
}
