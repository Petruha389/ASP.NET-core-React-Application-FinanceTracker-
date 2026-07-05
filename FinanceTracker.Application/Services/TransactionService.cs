using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinanceTracker.Application.DTOs.Transaction;
using FinanceTracker.Application.Exceptions;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Entities.Enums;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly FinanceTrackerDbContext _context;

    public TransactionService(FinanceTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsByAccountAsync(Guid accountId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .Include(t => t.Account)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();

        return transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.TransactionType,
            Description = t.Description,
            Date = t.CreatedDate,
            AccountId = t.AccountId,
            AccountName = t.Account?.Name
        });
    }

    public async Task<TransactionResponseDto> GetTransactionByIdAsync(Guid id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            throw new NotFoundException(nameof(Transaction), id);

        return new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.TransactionType,
            Description = transaction.Description,
            Date = transaction.CreatedDate,
            AccountId = transaction.AccountId,
            AccountName = transaction.Account?.Name
        };
    }

    public async Task<TransactionResponseDto> CreateTransactionAsync(TransactionCreateDto dto)
    {
        if (dto.Amount <= 0)
            throw new BusinessException("Сумма должна быть положительной.");

        var account = await _context.Accounts.FindAsync(dto.AccountId);
        if (account == null)
            throw new NotFoundException(nameof(Account), dto.AccountId);

        // Создаём транзакцию
        var transaction = new Transaction(dto.Amount, dto.Type, dto.Description, dto.AccountId);

        // Обновляем баланс
        if (dto.Type == TransactionType.Income)
            account.Deposit(dto.Amount);
        else if (dto.Type == TransactionType.Expense)
            account.Withdraw(dto.Amount);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.TransactionType,
            Description = transaction.Description,
            Date = transaction.CreatedDate,
            AccountId = transaction.AccountId,
            AccountName = account.Name
        };
    }

    public async Task UpdateTransactionAsync(Guid id, TransactionUpdateDto dto)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null)
            throw new NotFoundException(nameof(Transaction), id);

        // Обновляем только описание (без изменения баланса)
        transaction.UpdateDescription(dto.Description);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(Guid id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            throw new NotFoundException(nameof(Transaction), id);

        // Восстанавливаем баланс при удалении
        var account = transaction.Account;
        if (account != null)
        {
            if (transaction.TransactionType == TransactionType.Income)
                account.Withdraw(transaction.Amount); // отменяем доход
            else if (transaction.TransactionType == TransactionType.Expense)
                account.Deposit(transaction.Amount); // отменяем расход
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<TransactionResponseDto>> GetAllTransactionsAsync(
    Guid? userId = null,
    Guid? accountId = null,
    DateTime? from = null,
    DateTime? to = null)
    {
        var query = _context.Transactions
            .Include(t => t.Account)
                .ThenInclude(a => a.User)
            .AsQueryable();

        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);

        if (userId.HasValue)
            query = query.Where(t => t.Account.UserId == userId.Value);

        if (from.HasValue)
            query = query.Where(t => t.CreatedDate >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.CreatedDate <= to.Value);

        var transactions = await query
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();

        return transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.TransactionType,
            Description = t.Description,
            Date = t.CreatedDate,
            AccountId = t.AccountId,
            AccountName = t.Account?.Name,
            // Добавляем имя пользователя, чтобы видеть в таблице
            UserName = t.Account?.User?.Name
        });
    }

    public async Task<IEnumerable<TransactionResponseDto>> GetRecentTransactionsAsync(Guid userId, int count = 5)
    {
        // Получаем все ID счетов пользователя
        var userAccountIds = await _context.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .ToListAsync();

        if (!userAccountIds.Any())
            return Enumerable.Empty<TransactionResponseDto>();

        // Загружаем последние транзакции с учётом навигационных свойств
        var transactions = await _context.Transactions
            .Where(t => userAccountIds.Contains(t.AccountId))
            .Include(t => t.Account)
                .ThenInclude(a => a.User)
            .OrderByDescending(t => t.CreatedDate)
            .Take(count)
            .ToListAsync();

        // Маппинг в DTO
        return transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.TransactionType,
            Description = t.Description,
            Date = t.CreatedDate,
            AccountId = t.AccountId,
            AccountName = t.Account?.Name,
            UserName = t.Account?.User?.Name // если нужно для админ-дашборда
        });
    }
}