using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinanceTracker.Application.DTOs.Transaction;

namespace FinanceTracker.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionResponseDto>> GetTransactionsByAccountAsync(Guid accountId);
    Task<TransactionResponseDto> GetTransactionByIdAsync(Guid id);
    Task<TransactionResponseDto> CreateTransactionAsync(TransactionCreateDto dto);
    Task UpdateTransactionAsync(Guid id, TransactionUpdateDto dto);
    Task DeleteTransactionAsync(Guid id);
    Task<IEnumerable<TransactionResponseDto>> GetAllTransactionsAsync(
    Guid? userId = null,
    Guid? accountId = null,
    DateTime? from = null,
    DateTime? to = null);
    Task<IEnumerable<TransactionResponseDto>> GetRecentTransactionsAsync(Guid userId, int count = 5);
}