using System.ComponentModel.DataAnnotations;
using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Application.DTOs.User;

public class UserCreateDto
{
    [Required(ErrorMessage = "Имя обязательно")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    public string Password { get; set; } = string.Empty;
    public UserRole? Role { get; set; }
}