using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.Account;

public class AccountOperationDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше нуля")]
    public decimal Amount { get; set; }

    public string? Description { get; set; }
}