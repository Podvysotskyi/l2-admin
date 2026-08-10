namespace L2.Admin.Contracts;

public sealed record AccountSummary(
    Guid Id,
    string Username,
    string Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastSuccessfulLoginAt,
    bool HasActiveLoginSession);
