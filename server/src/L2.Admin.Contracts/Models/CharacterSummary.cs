namespace L2.Admin.Contracts;

public sealed record CharacterSummary(
    Guid Id,
    string Name,
    Guid AccountId,
    string? Username,
    int RaceId,
    string? RaceName,
    int SexId,
    string? SexName,
    int BaseClassId,
    string? BaseClassName,
    int ActiveClassId,
    string? ActiveClassName,
    short Level,
    long Experience,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DeleteAfter,
    string Status);
