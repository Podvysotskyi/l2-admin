namespace L2.Admin.Foundation;

public sealed class DependencyOptions
{
    public const string SectionName = "Dependencies";

    public bool PostgreSqlRequired { get; init; } = true;
}
