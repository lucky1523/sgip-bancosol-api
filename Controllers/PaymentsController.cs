using Microsoft.AspNetCore.Mvc;
using FinTech.API.Services;
using System.ComponentModel.DataAnnotations;

namespace FinTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ILoanService _loanService;

    public PaymentsController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<IActionResult> Pay([FromBody] PaymentRequest request)
    {
    var idempotencyKey = (string.IsNullOrWhiteSpace(request.IdempotencyKey) || request.IdempotencyKey == "string") 
                   ? Guid.NewGuid().ToString() 
                   : request.IdempotencyKey;

    try 
    {
        var success = await _loanService.RegisterPaymentAsync(request.ScheduleId, request.Amount, idempotencyKey);
        
        if (!success) return BadRequest("La cuota no existe o ya fue pagada.");

        return Ok(new { 
            Message = "Pago procesado exitosamente", 
            IdempotencyKey = idempotencyKey
        });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
    }
}

public record PaymentRequest(
    [Required] Guid ScheduleId, 
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")] decimal Amount, 
    string IdempotencyKey
);