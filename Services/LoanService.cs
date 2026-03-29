using FinTech.API.Data;
using FinTech.API.Models;
using FinTech.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace FinTech.API.Services;

public class LoanService : ILoanService
{
    private readonly ApplicationDbContext _context;

    public LoanService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<PaymentSchedule> SimulateLoan(decimal amount, decimal tea, int term)
    {
        var schedule = new List<PaymentSchedule>();
        var monthlyPayment = FinancialCalculator.CalculateMonthlyPayment(amount, tea, term);
        
        double teaDouble = (double)tea / 100;
        decimal tem = (decimal)(Math.Pow(1 + teaDouble, 1.0 / 12.0) - 1);

        decimal remainingBalance = amount;
        DateTime paymentDate = DateTime.UtcNow;

        for (int i = 1; i <= term; i++)
        {
            decimal interest = remainingBalance * tem;
            decimal principal = monthlyPayment - interest;
            remainingBalance -= principal;

            schedule.Add(new PaymentSchedule
            {
                PaymentNumber = i,
                DueDate = paymentDate.AddMonths(i),
                TotalPayment = monthlyPayment,
                Principal = Math.Round(principal, 2),
                Interest = Math.Round(interest, 2),
                RemainingBalance = Math.Max(0, Math.Round(remainingBalance, 2)),
                Status = PaymentStatus.Pending
            });
        }

        return schedule;
    }

    public async Task<Loan> CreateLoanAsync(decimal amount, decimal interestRate, int term)
    {
    var scheduleList = SimulateLoan(amount, interestRate, term);

    var loan = new Loan
    {
        Amount = amount,
        InterestRate = interestRate,
        Term = term,
        MonthlyPayment = scheduleList.FirstOrDefault()?.TotalPayment ?? 0, 
        PaymentSchedules = scheduleList
    };

    _context.Loans.Add(loan);
    await _context.SaveChangesAsync();

    return loan;
    }

    public async Task<bool> RegisterPaymentAsync(Guid scheduleId, decimal amount, string idempotencyKey)
    {
    var existingTransaction = await _context.Transactions
        .AnyAsync(t => t.IdempotencyKey == idempotencyKey);
    
    if (existingTransaction || idempotencyKey == "string") 
        throw new InvalidOperationException("Llave de idempotencia inválida o duplicada.");

    var schedule = await _context.PaymentSchedules
        .FirstOrDefaultAsync(s => s.Id == scheduleId);

    if (schedule == null || schedule.Status != 0) 
        return false;

    if (Math.Abs(schedule.TotalPayment - amount) > 0.01m)
        throw new InvalidOperationException($"El monto enviado ({amount}) no coincide con el de la cuota ({schedule.TotalPayment}).");

    schedule.Status = PaymentStatus.Paid; 

    var transaction = new Transaction
    {
        IdempotencyKey = idempotencyKey,
        Type = TransactionType.Payment,
        Amount = amount,
        Status = TransactionStatus.Completed,
        LoanId = schedule.LoanId,
        Description = $"Pago exitoso de cuota #{schedule.PaymentNumber}"
    };

    _context.Transactions.Add(transaction);
    await _context.SaveChangesAsync();
    return true;
    } 

    public async Task<List<Loan>> GetAllLoansAsync()
    {
    return await _context.Loans.OrderByDescending(l => l.CreatedAt).ToListAsync();
    }

    public async Task<Loan?> GetLoanByIdAsync(Guid id)
    {
    return await _context.Loans
        .Include(l => l.PaymentSchedules.OrderBy(ps => ps.PaymentNumber))
        .FirstOrDefaultAsync(l => l.Id == id);
    }
}