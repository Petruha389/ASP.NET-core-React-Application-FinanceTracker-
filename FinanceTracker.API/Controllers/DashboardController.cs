using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ITransactionService _transactionService;

    public DashboardController(IAccountService accountService, ITransactionService transactionService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var accounts = await _accountService.GetAccountsByUserAsync(userId);
        var totalBalance = accounts.Sum(a => a.Balance);
        var recentTransactions = await _transactionService.GetRecentTransactionsAsync(userId, 5);

        ViewBag.TotalBalance = totalBalance;
        ViewBag.AccountCount = accounts.Count();
        ViewBag.RecentTransactions = recentTransactions;

        return View();
    }
}