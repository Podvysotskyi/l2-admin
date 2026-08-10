using L2.Admin.Repositories.Contracts;
using SqlKata;
using SqlKata.Compilers;
using Xunit;

namespace L2.Admin.Repositories.Tests;

public sealed class AccountDirectoryRepositoryTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void BuildQuery_includes_login_projections_and_stable_ordering()
    {
        var query = AccountDirectoryRepository.BuildQuery(new Query("accounts as account"), string.Empty, Now);

        var result = new PostgresCompiler().Compile(query);

        Assert.Contains("account_login_history", result.Sql, StringComparison.Ordinal);
        Assert.Contains("account_sessions", result.Sql, StringComparison.Ordinal);
        Assert.Contains("LastSuccessfulLoginAt", result.Sql, StringComparison.Ordinal);
        Assert.Contains("ActiveLoginSessionCount", result.Sql, StringComparison.Ordinal);
        Assert.Contains("ORDER BY", result.Sql, StringComparison.Ordinal);
        Assert.Contains(Now, result.Bindings);
    }

    [Fact]
    public void BuildQuery_escapes_wildcard_characters()
    {
        var query = AccountDirectoryRepository.BuildQuery(new Query("accounts as account"), "%_\\", Now);

        var result = new PostgresCompiler().Compile(query);

        Assert.Contains("normalized_username", result.Sql, StringComparison.Ordinal);
        Assert.Contains("normalized_email", result.Sql, StringComparison.Ordinal);
        Assert.Contains(result.Bindings, binding =>
            binding is string value && value.Contains("\\%\\_\\\\", StringComparison.Ordinal));
    }

    [Fact]
    public void MapAccount_maps_timestamps_and_active_session_state()
    {
        var row = new AccountRow
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Username = "Player",
            Email = "player@example.com",
            CreatedAt = new DateTime(2026, 8, 9, 12, 0, 0, DateTimeKind.Utc),
            LastSuccessfulLoginAt = new DateTime(2026, 8, 10, 11, 0, 0, DateTimeKind.Utc),
            ActiveLoginSessionCount = 1
        };

        var account = AccountDirectoryRepository.MapAccount(row);

        Assert.Equal(row.Id, account.Id);
        Assert.Equal(new DateTimeOffset(row.CreatedAt), account.CreatedAt);
        Assert.Equal(new DateTimeOffset(row.LastSuccessfulLoginAt.Value), account.LastSuccessfulLoginAt);
        Assert.True(account.HasActiveLoginSession);
    }
}
