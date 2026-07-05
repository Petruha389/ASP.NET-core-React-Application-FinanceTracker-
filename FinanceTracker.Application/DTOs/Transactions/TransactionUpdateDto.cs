namespace FinanceTracker.Application.DTOs.Transaction;

public class TransactionUpdateDto
{
    public decimal Amount { get; set; }          // можно изменить сумму (но обычно не меняют)
    public string Description { get; set; } = string.Empty;
    // Type обычно не меняют, но можно добавить при необходимости
}