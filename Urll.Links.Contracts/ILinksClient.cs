using Urll.Links.Contracts.Dto;

namespace Urll.Links.Contracts;

public interface ILinksClient
{
    Task<LinkDto[]> GetAll();

    Task<LinkDto> Get(string code);

    Task Add(LinkAddDto link);

    Task Delete(string code);
}
