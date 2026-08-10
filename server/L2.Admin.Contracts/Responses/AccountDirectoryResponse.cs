namespace L2.Admin.Contracts;

public sealed record AccountDirectoryResponse(
    IReadOnlyList<AccountSummary> Items,
    long Total,
    int Page,
    int PageSize);
