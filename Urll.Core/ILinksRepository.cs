namespace Urll.Core;

public interface ILinksRepository
{
    public Task<bool> Add(Link link);

    public Task<Link?> GetOrDefault(string code);
}
