using Refit;
using Urll.Links.Contracts.Dto;

namespace Urll.Links.Contracts;

public interface ILinksClient
{
    [Get("/api/links")]
    Task<IApiResponse<LinkDto[]>> GetAll();

    [Get("/api/links/{code}")]
    Task<IApiResponse<LinkDto>> Get(string code);

    [Post("/api/links")]
    Task<IApiResponse<LinkDto>> Add(LinkAddDto link);

    [Delete("/api/links/{code}")]
    Task<IApiResponse> Delete(string code);
}
