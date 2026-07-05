using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTracker.Application.DTOs.Account;
using FinanceTracker.Application.Exceptions;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Entities.Enums;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.Services;

public class AccountService : IAccountService
{
    private readonly FinanceTrackerDbContext _context;

    public AccountService(FinanceTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountResponseDto>> GetAccountsByUserAsync(Guid userId)
    {
        var accounts = await _context.Accounts
            .Where(a => a.UserId == userId)
            .Include(a => a.Transactions)
            .ToListAsync();

        return accounts.Select(a => new AccountResponseDto
        {
            Id = a.Id,
            Name = a.Name,
            Type = a.AccountType,
            Balance = a.Balance,
            UserId = a.UserId,
            CreatedAt = a.CreatedAt,
            UserName = a.User?.Name
        });
    }

    public async Task<AccountResponseDto> GetAccountByIdAsync(Guid id)
    {
        var account = await _context.Accounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (account == null)
            throw new NotFoundException(nameof(Account), id);

        return new AccountResponseDto
        {
            Id = account.Id,
            Name = account.Name,
            Type = account.AccountType,
            Balance = account.Balance,
            UserId = account.UserId,
            CreatedAt = account.CreatedAt,
            UserName = account.User?.Name
        };
    }

    public async Task<AccountResponseDto> CreateAccountAsync(AccountCreateDto dto)
    {
        // Проверяем, что UserId заполнен (контроллер всегда должен его установить)
        if (!dto.UserId.HasValue)
            throw new BusinessException("UserId не может быть пустым.");

        var userId = dto.UserId.Value;

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new NotFoundException($"Пользователь с Id {userId} не найден.");

        var account = new Account(dto.Name, dto.Type, userId);
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountResponseDto
        {
            Id = account.Id,
            Name = account.Name,
            Type = account.AccountType,
            Balance = account.Balance,
            UserId = account.UserId,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task UpdateAccountAsync(Guid id, AccountUpdateDto dto)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            throw new NotFoundException(nameof(Account), id);

        account.UpdateDetails(dto.Name, dto.Type);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(Guid id)
    {
        var account = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (account == null)
            throw new NotFoundException(nameof(Account), id);

        if (account.Transactions.Any())
        {
            _context.Transactions.RemoveRange(account.Transactions);
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetBalanceAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new NotFoundException(nameof(Account), accountId);

        return account.Balance;
    }

    public async Task DepositAsync(Guid accountId, decimal amount, string description)
    {
        if (amount <= 0) throw new BusinessException("Сумма должна быть положительной.");
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null) throw new NotFoundException(nameof(Account), accountId);
        account.Deposit(amount);
        var transaction = new Transaction(amount, TransactionType.Income, description, accountId);
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task WithdrawAsync(Guid accountId, decimal amount, string description)
    {
        if (amount <= 0) throw new BusinessException("Сумма должна быть положительной.");
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null) throw new NotFoundException(nameof(Account), accountId);
        if (account.Balance < amount) throw new BusinessException("Недостаточно средств.");
        account.Withdraw(amount);
        var transaction = new Transaction(amount, TransactionType.Expense, description, accountId);
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync()
    {
        var accounts = await _context.Accounts
            .Include(a => a.User)
            .ToListAsync();

        return accounts.Select(a => new AccountResponseDto
        {
            Id = a.Id,
            Name = a.Name,
            Type = a.AccountType,
            Balance = a.Balance,
            UserId = a.UserId,
            CreatedAt = a.CreatedAt,
            UserName = a.User?.Name
        });
    }
}