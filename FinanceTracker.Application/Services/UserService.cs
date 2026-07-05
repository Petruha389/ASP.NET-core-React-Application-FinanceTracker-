using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceTracker.Application.DTOs.User;
using FinanceTracker.Application.Exceptions;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Application.Services;

public class UserService : IUserService
{
    private readonly FinanceTrackerDbContext _context;

    public UserService(FinanceTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new NotFoundException(nameof(User), id);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserResponseDto> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new NotFoundException($"Пользователь с email '{email}' не найден.");

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserResponseDto> CreateUserAsync(UserCreateDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new BusinessException("Пользователь с таким email уже существует.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User(dto.Name, dto.Email, passwordHash); // роль по умолчанию User

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task UpdateUserAsync(Guid id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new NotFoundException(nameof(User), id);

        user.UpdateProfile(dto.Name, dto.Email, dto.Role);

        if (!string.IsNullOrEmpty(dto.Password))
        {
            var newHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.ChangePassword(newHash);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new NotFoundException(nameof(User), id);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        try
        {

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException(nameof(User), userId);

            bool isValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);
            if (!isValid)
                throw new BusinessException("Неверный текущий пароль.");

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ChangePassword(newHash);

            await _context.SaveChangesAsync();
        } catch(Exception ex) {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}