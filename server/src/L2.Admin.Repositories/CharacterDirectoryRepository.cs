using L2.Admin.Contracts;
using L2.Admin.Exceptions;
using L2.Admin.Repositories.Contracts;
using L2.Admin.Repositories.Interfaces;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace L2.Admin.Repositories;

public sealed class CharacterDirectoryRepository(
    NpgsqlDataSource dataSource,
    TimeProvider timeProvider) : ICharacterDirectoryRepository
{
    public async Task<CharacterDirectoryResponse> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = query.ToUpperInvariant();
        var now = timeProvider.GetUtcNow();

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            using var database = new QueryFactory(connection, new PostgresCompiler());
            var directoryQuery = BuildQuery(
                database.Query("player.characters as character"),
                normalizedQuery);

            var result = await directoryQuery.PaginateAsync<CharacterRow>(
                page,
                pageSize,
                cancellationToken: cancellationToken);
            var characters = result.List.Select(character => MapCharacter(character, now)).ToArray();
            return new CharacterDirectoryResponse(characters, result.Count, page, pageSize);
        }
        catch (NpgsqlException exception)
        {
            throw new AdminRepositoryException("Character directory query failed.", exception);
        }
    }

    internal static Query BuildQuery(Query directoryQuery, string normalizedQuery)
    {
        directoryQuery
            .LeftJoin("accounts as account", "account.id", "character.account_id")
            .LeftJoin("content.player_races as race", "race.id", "character.player_race_id")
            .LeftJoin("content.player_sexes as sex", "sex.id", "character.player_sex_id")
            .LeftJoin("content.player_classes as base_class", join => join
                .On("base_class.id", "character.base_class_id")
                .On("base_class.player_race_id", "character.player_race_id")
                .On("base_class.player_sex_id", "character.player_sex_id"))
            .LeftJoin("content.player_classes as active_class", join => join
                .On("active_class.id", "character.active_class_id")
                .On("active_class.player_race_id", "character.player_race_id")
                .On("active_class.player_sex_id", "character.player_sex_id"))
            .Select(
                "character.id as Id",
                "character.name as Name",
                "character.account_id as AccountId",
                "account.username as Username",
                "character.player_race_id as RaceId",
                "race.name as RaceName",
                "character.player_sex_id as SexId",
                "sex.name as SexName",
                "character.base_class_id as BaseClassId",
                "base_class.name as BaseClassName",
                "character.active_class_id as ActiveClassId",
                "active_class.name as ActiveClassName",
                "character.level as Level",
                "character.experience as Experience",
                "character.created_at as CreatedAt",
                "character.delete_after as DeleteAfter")
            .OrderByDesc("character.created_at")
            .OrderByDesc("character.id");

        if (normalizedQuery.Length > 0)
        {
            var escapedQuery = EscapeLikePattern(normalizedQuery);
            directoryQuery.Where(filter => filter
                .WhereContains("character.normalized_name", escapedQuery, true, "\\")
                .OrWhereContains("account.normalized_username", escapedQuery, true, "\\")
                .OrWhereContains("account.normalized_email", escapedQuery, true, "\\"));
        }

        return directoryQuery;
    }

    internal static CharacterSummary MapCharacter(CharacterRow character, DateTimeOffset now)
    {
        var deleteAfter = character.DeleteAfter is null
            ? (DateTimeOffset?)null
            : new DateTimeOffset(character.DeleteAfter.Value);
        return new CharacterSummary(
            character.Id,
            character.Name,
            character.AccountId,
            character.Username,
            character.RaceId,
            character.RaceName,
            character.SexId,
            character.SexName,
            character.BaseClassId,
            character.BaseClassName,
            character.ActiveClassId,
            character.ActiveClassName,
            character.Level,
            character.Experience,
            new DateTimeOffset(character.CreatedAt),
            deleteAfter,
            deleteAfter is null
                ? "active"
                : deleteAfter <= now ? "deletion_expired" : "pending_deletion");
    }

    private static string EscapeLikePattern(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("%", "\\%", StringComparison.Ordinal)
        .Replace("_", "\\_", StringComparison.Ordinal);
}
