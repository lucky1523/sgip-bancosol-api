namespace FinTech.API.Data;
using Microsoft.EntityFrameworkCore;
using FinTech.API.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Loan> Loans { get; set; }
    public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.IdempotencyKey)
            .IsUnique();

        // relacion
        modelBuilder.Entity<PaymentSchedule>()
            .HasOne<Loan>()
            .WithMany(l => l.PaymentSchedules)
            .HasForeignKey(p => p.LoanId);
    }
}