namespace L2.Admin.Repositories.Contracts;

internal sealed class CharacterRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GameVersion { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public string? Username { get; set; }
    public int RaceId { get; set; }
    public string? RaceName { get; set; }
    public int SexId { get; set; }
    public string? SexName { get; set; }
    public int BaseClassId { get; set; }
    public string? BaseClassName { get; set; }
    public int ActiveClassId { get; set; }
    public string? ActiveClassName { get; set; }
    public short Level { get; set; }
    public long Experience { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeleteAfter { get; set; }
}
