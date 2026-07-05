using FinanceTracker.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.User;

public class UserUpdateDto
{
    [Required(ErrorMessage = "Имя обязательно")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    // Пароль опционален – если не указан, не меняем
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Роль обязательна")]
    public UserRole Role { get; set; } = UserRole.User;
}