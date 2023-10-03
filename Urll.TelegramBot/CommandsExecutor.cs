using System.Text;
using Refit;
using Urll.Links.Contracts;
using Urll.Links.Contracts.Dto;

namespace Urll.TelegramBot;

public class CommandsExecutor
{
    public CommandsExecutor(ILinksClient linksClient)
    {
        _linksClient = linksClient;
    }

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

        IApiResponse<LinkDto[]> apiResponse = await _linksClient.GetAll();
        if (!apiResponse.IsSuccessStatusCode || apiResponse.Content is null)
        {
            return GetErrorMessage(apiResponse);
        }

        StringBuilder builder = new();
        foreach (LinkDto link in apiResponse.Content)
        {
            builder.AppendLine($"{link.Code}: {link.Url}");
        }

        return builder.ToString();
    }

    private async Task<string> ExecuteGetCommand(string[] args)
    {
        if (args.Length != 1)
        {
            return GetHelpText();
        }

        IApiResponse<LinkDto> apiResponse = await _linksClient.Get(args[0]);
        if (!apiResponse.IsSuccessStatusCode || apiResponse.Content is null)
        {
            return GetErrorMessage(apiResponse);
        }

        return apiResponse.Content.Url;
    }

    private async Task<string> ExecuteDeleteCommand(string[] args)
    {
        if (args.Length != 1)
        {
            return GetHelpText();
        }

        IApiResponse apiResponse = await _linksClient.Delete(args[0]);
        if (!apiResponse.IsSuccessStatusCode)
        {
            return GetErrorMessage(apiResponse);
        }

        return "Link deleted";
    }

    private async Task<string> ExecuteAddCommand(string[] args)
    {
        if (args.Length != 2)
        {
            return GetHelpText();
        }

        LinkAddDto dto = new(args[0], args[1]);
        IApiResponse apiResponse = await _linksClient.Add(dto);
        if (!apiResponse.IsSuccessStatusCode)
        {
            return GetErrorMessage(apiResponse);
        }

        return "Link added";
    }

    private string GetErrorMessage(IApiResponse apiResponse)
    {
        return apiResponse.Error?.Message ?? "Command failed";
    }

    private readonly ILinksClient _linksClient;
}
