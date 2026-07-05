using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinanceTracker.Application.DTOs.User;
using FinanceTracker.Application.Exceptions;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly FinanceTrackerDbContext _context;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthService(FinanceTrackerDbContext context, IUserService userService, IConfiguration configuration)
    {
        _context = context;
        _userService = userService;
        _configuration = configuration;
    }

    public async Task<UserResponseDto> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new BusinessException("Неверный email или пароль.");

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isValid)
            throw new BusinessException("Неверный email или пароль.");

        // Возвращаем DTO (чтобы не возвращать сущность напрямую)
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        // Проверка, что пароли совпадают (это также проверяется атрибутом [Compare] в DTO,
        // но дублируем для безопасности)
        if (dto.Password != dto.ConfirmPassword)
            throw new BusinessException("Пароли не совпадают.");

        // Используем UserService для создания пользователя, чтобы не дублировать логику
        // преобразуем UserRegisterDto в UserCreateDto (у них одинаковые поля, но разные названия)
        var createDto = new UserCreateDto
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password
        };

        // UserService сам проверит уникальность email и захеширует пароль
        var createdUser = await _userService.CreateUserAsync(createDto);
        return createdUser;
    }

    public async Task<string> GenerateJwtTokenAsync(UserResponseDto userDto)
    {
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userDto.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
        new Claim(ClaimTypes.Name, userDto.Name),
        new Claim(ClaimTypes.Role, userDto.Role.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
