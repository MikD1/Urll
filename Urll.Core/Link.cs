namespace Urll.Core;

public class Link
{
    public Link(string url, string code)
    {
        Created = DateTime.Now;
        Url = url;
        Code = code;
    }

    public DateTime Created { get; }

    public string Url { get; }

    public string Code { get; }
}
