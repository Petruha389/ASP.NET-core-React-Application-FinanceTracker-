using FinanceTracker.Application.DTOs.User;
using FinanceTracker.Application.DTOs.Account;
using FinanceTracker.Application.DTOs.Transaction;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly ITransactionService _transactionService;

    public AdminController(IUserService userService, IAccountService accountService, ITransactionService transactionService)
    {
        _userService = userService;
        _accountService = accountService;
        _transactionService = transactionService;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        return View(new UserCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(UserCreateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            await _userService.CreateUserAsync(dto);
            TempData["Success"] = "Пользователь создан";
            return RedirectToAction(nameof(Users));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return View(new UserUpdateDto { Name = user.Name, Email = user.Email, Role = user.Role });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(Guid id, UserUpdateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            await _userService.UpdateUserAsync(id, dto);
            TempData["Success"] = "Пользователь обновлён";
            return RedirectToAction(nameof(Users));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            TempData["Success"] = "Пользователь удалён";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Accounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return View(accounts);
    }

    public async Task<IActionResult> Transactions(DateTime? from, DateTime? to, Guid? userId, Guid? accountId)
    {
        var transactions = await _transactionService.GetAllTransactionsAsync(userId, accountId, from, to);
        ViewBag.Users = await _userService.GetAllUsersAsync();
        ViewBag.Accounts = await _accountService.GetAllAccountsAsync();
        return View(transactions);
    }
}