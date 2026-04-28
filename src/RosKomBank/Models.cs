using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RosKomBank;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string PassportNumber { get; set; } = "";
    public string INN { get; set; } = "";
    public string Address { get; set; } = "";
    public string PinHash { get; set; } = "";
    public decimal Balance { get; set; }
    public decimal CreditBalance { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal MonthlyPayment { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public decimal MatCapitalBalance { get; set; }
    public bool HasMatCapital { get; set; }
    public decimal TaxDebt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
    public List<Fine> Fines { get; set; } = new();
}

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Type { get; set; } = "";
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string? RecipientPhone { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Fine
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string FineNumber { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class BankContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Fine> Fines { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder o)
        => o.UseSqlite("Data Source=roskombank.db");

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>().HasMany(u => u.Transactions).WithOne(t => t.User!).HasForeignKey(t => t.UserId);
        mb.Entity<User>().HasMany(u => u.Fines).WithOne(f => f.User!).HasForeignKey(f => f.UserId);
    }
}
