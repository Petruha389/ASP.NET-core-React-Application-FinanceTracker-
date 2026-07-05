using FinanceTracker.Domain.Entities.Enums;
using FinanceTracker.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public ICollection<Account> Accounts { get; private set; } = new List<Account>();

    // Конструктор для создания
    public User(string name, string email, string passwordHash, UserRole role = UserRole.User)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    // Метод для обновления профиля (без пароля)
    public void UpdateProfile(string name, string email, UserRole role)
    {
        Name = name;
        Email = email;
        Role = role;
    }

    // Метод для смены пароля
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    // Приватный конструктор для EF (если нужен)
    private User() { }
}