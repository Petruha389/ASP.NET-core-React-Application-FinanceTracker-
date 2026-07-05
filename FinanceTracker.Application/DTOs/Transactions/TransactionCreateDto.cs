using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Application.DTOs.Transaction;

public class TransactionCreateDto
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
}