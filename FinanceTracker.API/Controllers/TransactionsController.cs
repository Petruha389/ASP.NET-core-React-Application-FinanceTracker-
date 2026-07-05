using FinanceTracker.Application.DTOs.Transaction;
using FinanceTracker.Application.Exceptions;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public TransactionsController(ITransactionService transactionService, IAccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(Guid? accountId)
    {
        if (accountId == null)
        {
            // Если не указан счет, показать все транзакции пользователя (сложно, лучше попросить выбрать счет)
            // Для простоты покажем только транзакции первого счета или вернем ошибку.
            return RedirectToAction("Index", "Accounts");
        }

        // Проверка, что счет принадлежит пользователю
        var account = await _accountService.GetAccountByIdAsync(accountId.Value);
        if (account.UserId != GetUserId())
            return Forbid();

        var transactions = await _transactionService.GetTransactionsByAccountAsync(accountId.Value);
        return View(transactions);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id.Value);
            // Проверка владельца счета через аккаунт
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();
            return View(transaction);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Create(Guid? accountId)
    {
        if (accountId == null)
            return RedirectToAction("Index", "Accounts");

        var account = await _accountService.GetAccountByIdAsync(accountId.Value);
        if (account.UserId != GetUserId())
            return Forbid();

        var dto = new TransactionCreateDto { AccountId = accountId.Value };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransactionCreateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            // Проверка владельца счета
            var account = await _accountService.GetAccountByIdAsync(dto.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();

            await _transactionService.CreateTransactionAsync(dto);
            return RedirectToAction(nameof(Index), new { accountId = dto.AccountId });
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id.Value);
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();

            var dto = new TransactionUpdateDto
            {
                Description = transaction.Description
            };
            return View(dto);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TransactionUpdateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();

            await _transactionService.UpdateTransactionAsync(id, dto);
            return RedirectToAction(nameof(Index), new { accountId = transaction.AccountId });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id.Value);
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();
            return View(transaction);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();

            await _transactionService.DeleteTransactionAsync(id);
            return RedirectToAction(nameof(Index), new { accountId = transaction.AccountId });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}