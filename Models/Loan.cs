using System.ComponentModel.DataAnnotations;

namespace FinTech.API.Models;

public enum LoanType { Fixed, Decreasing }
public enum LoanStatus { Pending, Approved, Rejected, Active }

public class Loan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string UserId { get; set; } = "user-123";
    [Range(500, 50000)]
    public decimal Amount { get; set; }
    [Range(6, 60)]
    public int Term { get; set; } // en meses
    public decimal InterestRate { get; set; } 
    public LoanType Type { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Pending;
    public decimal MonthlyPayment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relación con el cronograma
    public List<PaymentSchedule> PaymentSchedules { get; set; } = new();
}
