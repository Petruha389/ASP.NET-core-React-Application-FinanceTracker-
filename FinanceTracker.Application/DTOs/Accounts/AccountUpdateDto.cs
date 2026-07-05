using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Application.DTOs.Account;

public class AccountUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
}