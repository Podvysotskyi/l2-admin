namespace L2.Admin.Contracts;

public sealed record GameVersionSummary(
    string Key,
    string DisplayName,
    int SortOrder,
    bool IsDefault);
