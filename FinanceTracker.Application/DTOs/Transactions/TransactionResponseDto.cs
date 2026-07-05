using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Application.DTOs.Transaction;

public class TransactionResponseDto
{
    public required Guid Id { get; set; }
    public required decimal Amount { get; set; }
    public required TransactionType Type { get; set; }
    public required string Description { get; set; }
    public required DateTimeOffset Date { get; set; }
    public required Guid AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? UserName { get; set; }
}