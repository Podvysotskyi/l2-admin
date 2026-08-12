using L2.Admin.Repositories.Contracts;
using SqlKata;
using SqlKata.Compilers;
using Xunit;

namespace L2.Admin.Repositories.Tests;

public sealed class CharacterDirectoryRepositoryTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void BuildQuery_includes_content_and_account_joins()
    {
        var query = CharacterDirectoryRepository.BuildQuery(
            new Query("player.characters as character"),
            "c1",
            "OWNER@EXAMPLE.COM");

        var result = new PostgresCompiler().Compile(query);

        Assert.Contains("accounts", result.Sql, StringComparison.Ordinal);
        Assert.Contains("player_races", result.Sql, StringComparison.Ordinal);
        Assert.Contains("player_sexes", result.Sql, StringComparison.Ordinal);
        Assert.Contains("player_classes", result.Sql, StringComparison.Ordinal);
        Assert.Contains("normalized_name", result.Sql, StringComparison.Ordinal);
        Assert.Contains("normalized_email", result.Sql, StringComparison.Ordinal);
        Assert.Contains(result.Bindings, binding => Equals(binding, "c1"));
        Assert.Contains(result.Bindings, binding => Equals(binding, "%OWNER@EXAMPLE.COM%"));
    }

    [Fact]
    public void MapCharacter_maps_labels_and_active_status()
    {
        var row = CreateRow();

        var character = CharacterDirectoryRepository.MapCharacter(row, Now);

        Assert.Equal(row.Id, character.Id);
        Assert.Equal("Human", character.RaceName);
        Assert.Equal("Male", character.SexName);
        Assert.Equal("Fighter", character.BaseClassName);
        Assert.Equal("Warrior", character.ActiveClassName);
        Assert.Equal("active", character.Status);
    }

    [Fact]
    public void MapCharacter_distinguishes_expired_and_pending_deletion()
    {
        var expiredRow = CreateRow();
        expiredRow.DeleteAfter = new DateTime(2026, 8, 10, 11, 0, 0, DateTimeKind.Utc);
        var pendingRow = CreateRow();
        pendingRow.DeleteAfter = new DateTime(2026, 8, 10, 13, 0, 0, DateTimeKind.Utc);

        var expired = CharacterDirectoryRepository.MapCharacter(expiredRow, Now);
        var pending = CharacterDirectoryRepository.MapCharacter(pendingRow, Now);

        Assert.Equal("deletion_expired", expired.Status);
        Assert.Equal("pending_deletion", pending.Status);
    }

    private static CharacterRow CreateRow() => new()
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
        Name = "Hero",
        GameVersion = "c1",
        AccountId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
        Username = "Owner",
        RaceId = 0,
        RaceName = "Human",
        SexId = 0,
        SexName = "Male",
        BaseClassId = 10,
        BaseClassName = "Fighter",
        ActiveClassId = 11,
        ActiveClassName = "Warrior",
        Level = 20,
        Experience = 1000,
        CreatedAt = new DateTime(2026, 8, 10, 10, 0, 0, DateTimeKind.Utc)
    };
}
