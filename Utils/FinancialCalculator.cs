namespace FinTech.API.Utils;

public static class FinancialCalculator
{
    //Sistema Francés.
    public static decimal CalculateMonthlyPayment(decimal amount, decimal tea, int term)
    {
        double teaDouble = (double)tea / 100;
        double tem = Math.Pow(1 + teaDouble, 1.0 / 12.0) - 1;

        double p = (double)amount;
        double n = term;
        
        double monthlyPayment = p * (tem * Math.Pow(1 + tem, n)) / (Math.Pow(1 + tem, n) - 1);

        return (decimal)Math.Round(monthlyPayment, 2);
    }
}