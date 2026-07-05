using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FinanceTracker.Infrastructure.Data;

public class FinanceTrackerDbContext : DbContext
{
    public FinanceTrackerDbContext(
        DbContextOptions<FinanceTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public static async Task SeedAsync(FinanceTrackerDbContext context)
    {
        if (!await context.Users.AnyAsync(u => u.Email == "admin@example.com"))
        {
            var admin = new User(
                name: "Admin",
                email: "admin@example.com",
                passwordHash: BCrypt.Net.BCrypt.HashPassword("admin123"),
                role: UserRole.Admin
            );
            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Accounts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Account>()
            .HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId);
    }
}
