using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinanceTracker.Application.DTOs.User;

namespace FinanceTracker.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> GetUserByIdAsync(Guid id);
    Task<UserResponseDto> CreateUserAsync(UserCreateDto dto);
    Task UpdateUserAsync(Guid id, UserUpdateDto dto);
    Task DeleteUserAsync(Guid id);
    Task<UserResponseDto> GetUserByEmailAsync(string email);
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}
