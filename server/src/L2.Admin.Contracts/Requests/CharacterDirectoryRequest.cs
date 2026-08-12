namespace L2.Admin.Contracts;

public sealed class CharacterDirectoryRequest
{
    public string GameVersion { get; set; } = "interlude";
    public string? Query { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
