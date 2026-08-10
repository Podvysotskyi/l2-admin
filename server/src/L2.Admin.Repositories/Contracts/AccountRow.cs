namespace L2.Admin.Repositories.Contracts;

internal sealed class AccountRow
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSuccessfulLoginAt { get; set; }
    public long ActiveLoginSessionCount { get; set; }
}
