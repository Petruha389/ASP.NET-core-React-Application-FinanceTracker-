using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using FinanceTracker.Domain.Entities.Enums;

namespace FinanceTracker.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Currency Currency { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        public AccountType AccountType { get; private set;}
        public Account(string name, AccountType accountType, Guid userId)
        {
            Id = Guid.NewGuid();
            Name = name;
            AccountType = accountType;
            UserId = userId;
            Balance = 0;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        // Методы для изменения баланса
        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Сумма должна быть положительной.");
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Сумма должна быть положительной.");
            if (Balance < amount) throw new InvalidOperationException("Недостаточно средств.");
            Balance -= amount;
        }

        // Метод для обновления имени/типа (опционально)
        public void UpdateDetails(string name, AccountType accountType)
        {
            Name = name;
            AccountType = accountType;
        }

        // Приватный конструктор для EF
        private Account() { }
        public User User { get; private set; } = null!;

        public ICollection<Transaction> Transactions { get; private set; }
            = new List<Transaction>();
    }
}
