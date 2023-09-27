using System.Text.RegularExpressions;

namespace Urll.Core;

public class Link
{
    public static Link? Create(string url, string code, out string[] validationResult)
    {
        List<string> urlValidationResult = ValidateUrl(url);
        List<string> codeValidationResult = ValidateCode(code);
        validationResult = urlValidationResult.Concat(codeValidationResult).ToArray();
        if (validationResult.Length > 0)
        {
            return null;
        }

        return new Link(url, code);
    }

    public DateTime Created { get; }

    public string Url { get; }

    public string Code { get; }

    private static List<string> ValidateUrl(string url)
    {
        List<string> validationResults = new();

        if (string.IsNullOrEmpty(url))
        {
            validationResults.Add("Url cannot be empty.");
        }

        if (url.Length > 1000)
        {
            validationResults.Add("Url length cannot be greater than 1000.");
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            validationResults.Add("Invalid Url format.");
        }

        return validationResults;
    }

    private static List<string> ValidateCode(string code)
    {
        List<string> validationResults = new();

        if (string.IsNullOrEmpty(code))
        {
            validationResults.Add("Code cannot be empty.");
        }

        if (code.Length > 50)
        {
            validationResults.Add("Code length cannot be greater than 50.");
        }

        Regex regex = new("^[a-zA-Z0-9_-]*$");
        if (!regex.IsMatch(code))
        {
            validationResults.Add("Code can only contain alphanumeric characters, underscores, and dashes.");
        }

        return validationResults;
    }

    private Link(string url, string code)
    {
        Created = DateTime.Now;
        Url = url;
        Code = code;
    }
}
