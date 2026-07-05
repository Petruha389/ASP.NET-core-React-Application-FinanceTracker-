using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceTracker.Application.DTOs.User;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

using FinanceTracker.Application.DTOs.User;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<UserResponseDto> ValidateCredentialsAsync(string email, string password);
    Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
    Task<string> GenerateJwtTokenAsync(UserResponseDto userDto); // <-- параметр UserResponseDto
}