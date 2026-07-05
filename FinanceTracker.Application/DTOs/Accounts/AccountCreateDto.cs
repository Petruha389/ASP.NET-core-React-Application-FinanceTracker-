using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Application.DTOs.Account;

public class AccountCreateDto
{
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public Guid? UserId { get; set; }
}
