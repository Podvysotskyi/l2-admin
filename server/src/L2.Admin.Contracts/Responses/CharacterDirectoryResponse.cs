namespace L2.Admin.Contracts;

public sealed record CharacterDirectoryResponse(
    IReadOnlyList<CharacterSummary> Items,
    long Total,
    int Page,
    int PageSize);
