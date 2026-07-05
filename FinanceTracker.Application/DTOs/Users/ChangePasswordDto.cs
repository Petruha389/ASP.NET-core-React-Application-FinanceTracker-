using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.User;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Текущий пароль обязателен")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Новый пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтверждение пароля обязательно")]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}