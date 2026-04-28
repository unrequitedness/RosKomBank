using System;
using System.Collections.Generic;
using System.Linq;

namespace RosKomBank.Mobile;

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
}

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
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
    public string FineNumber { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

/// <summary>
/// In-memory data store. All state is held in static lists and resets on process restart.
/// </summary>
public static class Store
{
    public static readonly List<User> Users = new();
    public static readonly List<Transaction> Transactions = new();
    public static readonly List<Fine> Fines = new();

    private static int _userId, _txId, _fineId;
    public static int NextUserId() => ++_userId;
    public static int NextTxId() => ++_txId;
    public static int NextFineId() => ++_fineId;

    public static void AddUser(User u)
    {
        if (u.Id == 0) u.Id = NextUserId();
        Users.Add(u);
    }
    public static void AddTransaction(Transaction t)
    {
        if (t.Id == 0) t.Id = NextTxId();
        Transactions.Add(t);
    }
    public static void AddFine(Fine f)
    {
        if (f.Id == 0) f.Id = NextFineId();
        Fines.Add(f);
    }

    public static User? FindUserByPhonePin(string phone, string pin)
        => Users.FirstOrDefault(u => u.Phone == phone && u.PinHash == pin);
    public static User? FindUserByPhone(string phone)
        => Users.FirstOrDefault(u => u.Phone == phone);
    public static User? FindUserById(int id) => Users.FirstOrDefault(u => u.Id == id);
    public static IEnumerable<Transaction> UserTxs(int userId)
        => Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt);
    public static IEnumerable<Fine> UserFines(int userId)
        => Fines.Where(f => f.UserId == userId).OrderByDescending(f => f.IssuedAt);

    public static void SeedIfEmpty()
    {
        if (Users.Count > 0) return;
        var u = new User
        {
            FullName = "Иванов Иван Иванович",
            Phone = "79001234567",
            Email = "ivanov@mail.ru",
            PassportNumber = "4521 876543",
            INN = "772012345678",
            Address = "г. Москва, ул. Ленина, д. 1",
            PinHash = "1234",
            Balance = 125430.50m,
            CreditBalance = 50000m,
            CreditLimit = 100000m,
            MonthlyPayment = 4500m,
            NextPaymentDate = DateTime.Now.AddDays(15),
            MatCapitalBalance = 586946.72m,
            HasMatCapital = true,
            TaxDebt = 1240m,
            CreatedAt = DateTime.Now.AddYears(-2),
        };
        AddUser(u);
        AddTransaction(new Transaction { UserId = u.Id, Type = "topup", Amount = 15000, Description = "Пополнение счёта", Status = "success", CreatedAt = DateTime.Now.AddDays(-3) });
        AddTransaction(new Transaction { UserId = u.Id, Type = "transfer_out", Amount = -5000, Description = "Перевод Сидорову С.С.", Status = "success", RecipientPhone = "79007654321", CreatedAt = DateTime.Now.AddDays(-5) });
        AddTransaction(new Transaction { UserId = u.Id, Type = "credit", Amount = -4500, Description = "Платёж по кредиту", Status = "success", CreatedAt = DateTime.Now.AddDays(-20) });
        AddFine(new Fine { UserId = u.Id, FineNumber = "18810336240101234567", Description = "Превышение скорости 20-40 км/ч", Amount = 500, IsPaid = false, IssuedAt = DateTime.Now.AddDays(-30) });
        AddFine(new Fine { UserId = u.Id, FineNumber = "18810336240109876543", Description = "Нарушение правил стоянки", Amount = 1500, IsPaid = true, IssuedAt = DateTime.Now.AddDays(-60), PaidAt = DateTime.Now.AddDays(-50) });
    }
}
