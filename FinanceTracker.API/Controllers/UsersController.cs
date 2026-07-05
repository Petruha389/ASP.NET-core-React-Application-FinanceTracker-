using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.DTOs.User;
using System.Security.Claims;
using FinanceTracker.Application.Exceptions;

namespace FinanceTracker.API.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // ... существующие методы (Index, Details, Create, Edit, Delete)

    // ===== ПРОФИЛЬ =====
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userService.GetUserByIdAsync(userId);
        var dto = new UserUpdateDto
        {
            Name = user.Name,
            Email = user.Email,
            Role = user.Role // только для отображения, не редактируется
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(UserUpdateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Не позволяем менять роль через эту форму
        dto.Role = (await _userService.GetUserByIdAsync(userId)).Role;
        try
        {
            await _userService.UpdateUserAsync(userId, dto);
            TempData["Success"] = "Профиль обновлён";
            return RedirectToAction(nameof(EditProfile));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    // ===== СМЕНА ПАРОЛЯ =====
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
            TempData["Success"] = "Пароль изменён";
            return RedirectToAction(nameof(ChangePassword));
        }
        catch (NotFoundException)
        {
            ModelState.AddModelError("", "Пользователь не найден.");
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        catch (Exception ex)
        {
            // Логируем для отладки
            Console.WriteLine($"Ошибка смены пароля: {ex.Message}");
            ModelState.AddModelError("", "Внутренняя ошибка сервера.");
        }
        return View(dto);
    }
}