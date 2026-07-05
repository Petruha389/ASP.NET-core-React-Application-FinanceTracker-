using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Application.DTOs.User;

public class UserResponseDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required UserRole Role { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}