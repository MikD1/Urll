namespace Urll.Core;

public interface ILinksRepository
{
    public Task<IReadOnlyCollection<Link>> GetAll();

    public Task<bool> Add(Link link);

    public Task<Link?> GetOrDefault(string code);

    public Task<bool> Delete(string code);
}
