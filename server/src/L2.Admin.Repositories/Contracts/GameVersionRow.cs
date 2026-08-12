namespace L2.Admin.Repositories.Contracts;

internal sealed class GameVersionRow
{
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
