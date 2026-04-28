using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace RosKomBank;

public partial class App : Application
{
    public static User? CurrentUser { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            using var db = new BankContext();
            db.Database.EnsureCreated();
            if (!db.Users.Any())
            {
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
                db.Users.Add(u);
                db.SaveChanges();
                db.Transactions.AddRange(
                    new Transaction { UserId = u.Id, Type = "topup", Amount = 15000, Description = "Пополнение счёта", Status = "success", CreatedAt = DateTime.Now.AddDays(-3) },
                    new Transaction { UserId = u.Id, Type = "transfer_out", Amount = -5000, Description = "Перевод Сидорову С.С.", Status = "success", RecipientPhone = "79007654321", CreatedAt = DateTime.Now.AddDays(-5) },
                    new Transaction { UserId = u.Id, Type = "credit", Amount = -4500, Description = "Платёж по кредиту", Status = "success", CreatedAt = DateTime.Now.AddDays(-20) }
                );
                db.Fines.AddRange(
                    new Fine { UserId = u.Id, FineNumber = "18810336240101234567", Description = "Превышение скорости 20-40 км/ч", Amount = 500, IsPaid = false, IssuedAt = DateTime.Now.AddDays(-30) },
                    new Fine { UserId = u.Id, FineNumber = "18810336240109876543", Description = "Нарушение правил стоянки", Amount = 1500, IsPaid = true, IssuedAt = DateTime.Now.AddDays(-60), PaidAt = DateTime.Now.AddDays(-50) }
                );
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Ошибка БД: " + ex.Message);
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new LoginWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
