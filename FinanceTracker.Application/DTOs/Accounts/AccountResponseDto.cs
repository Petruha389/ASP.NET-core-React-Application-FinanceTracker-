using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Application.DTOs.Account;

public class AccountResponseDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required AccountType Type { get; set; }
    public required decimal Balance { get; set; }
    public required Guid UserId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public string? UserName { get; set; }
}