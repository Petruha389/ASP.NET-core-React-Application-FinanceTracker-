using FinanceTracker.Application.DTOs.Account;
using FinanceTracker.Application.Exceptions;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers;

[Authorize]
public class AccountsController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ITransactionService _transactionService;

    public AccountsController(IAccountService accountService, ITransactionService transactionService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET: Accounts
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var accounts = await _accountService.GetAccountsByUserAsync(userId);
        return View(accounts);
    }

    // GET: Accounts/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId()) return Forbid();
            return View(account);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // GET: Accounts/Create
    public IActionResult Create()
    {
        return View(new AccountCreateDto());
    }

    // POST: Accounts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AccountCreateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            dto.UserId = GetUserId();
            await _accountService.CreateAccountAsync(dto);
            TempData["Success"] = "Счёт создан";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    // GET: Accounts/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId()) return Forbid();
            return View(new AccountUpdateDto { Name = account.Name, Type = account.Type });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // POST: Accounts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AccountUpdateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            await _accountService.UpdateAccountAsync(id, dto);
            TempData["Success"] = "Счёт обновлён";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    // GET: Accounts/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId()) return Forbid();
            return View(account);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // POST: Accounts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _accountService.DeleteAccountAsync(id);
            TempData["Success"] = "Счёт удалён";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // ===== ПОПОЛНЕНИЕ =====
    [HttpGet]
    public async Task<IActionResult> Deposit(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId()) return Forbid();
            ViewBag.AccountName = account.Name;
            ViewBag.AccountId = id;
            return View(new AccountOperationDto());
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deposit(Guid id, AccountOperationDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AccountId = id;
            return View(dto);
        }
        try
        {
            await _accountService.DepositAsync(id, dto.Amount, dto.Description);
            TempData["Success"] = "Счёт пополнен";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.AccountId = id;
            return View(dto);
        }
    }

    // ===== СНЯТИЕ =====
    [HttpGet]
    public async Task<IActionResult> Withdraw(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId()) return Forbid();
            ViewBag.AccountName = account.Name;
            ViewBag.AccountId = id;
            return View(new AccountOperationDto());
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(Guid id, AccountOperationDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AccountId = id;
            return View(dto);
        }
        try
        {
            await _accountService.WithdrawAsync(id, dto.Amount, dto.Description);
            TempData["Success"] = "Средства сняты";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.AccountId = id;
            return View(dto);
        }
    }
}