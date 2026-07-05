using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinanceTracker.Application.DTOs.Account;

namespace FinanceTracker.Application.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<AccountResponseDto>> GetAccountsByUserAsync(Guid userId);
    Task<AccountResponseDto> GetAccountByIdAsync(Guid id);
    Task<AccountResponseDto> CreateAccountAsync(AccountCreateDto dto);
    Task UpdateAccountAsync(Guid id, AccountUpdateDto dto);
    Task DeleteAccountAsync(Guid id);
    Task<decimal> GetBalanceAsync(Guid accountId);
    Task DepositAsync(Guid accountId, decimal amount, string description);
    Task WithdrawAsync(Guid accountId, decimal amount, string description);
    Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync();
}
