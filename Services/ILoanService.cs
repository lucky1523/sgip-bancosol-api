using FinTech.API.Models;

namespace FinTech.API.Services;

public interface ILoanService
{
    List<PaymentSchedule> SimulateLoan(decimal amount, decimal tea, int term);
    
    Task<Loan> CreateLoanAsync(decimal amount, decimal interestRate, int term);

    Task<bool> RegisterPaymentAsync(Guid scheduleId, decimal amount, string idempotencyKey);

    Task<List<Loan>> GetAllLoansAsync();
    Task<Loan?> GetLoanByIdAsync(Guid id);
}