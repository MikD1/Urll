namespace Urll.Core;

public record class LinkDto(
    DateTime Created,
    string Url,
    string Code);
