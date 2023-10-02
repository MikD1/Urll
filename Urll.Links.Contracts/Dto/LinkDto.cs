namespace Urll.Links.Contracts.Dto;

public record class LinkDto(
    DateTime Created,
    string Url,
    string Code);
