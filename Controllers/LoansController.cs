using Microsoft.AspNetCore.Mvc;
using FinTech.API.Services;
using FinTech.API.Models;

namespace FinTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost("simulate")]
    public IActionResult Simulate([FromBody] SimulationRequest request)
    {
        var schedule = _loanService.SimulateLoan(request.Amount, request.Tea, request.Term);
        return Ok(schedule);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SimulationRequest request)
    {   
        var loan = await _loanService.CreateLoanAsync(request.Amount, request.Tea, request.Term);
    
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    // GET: api/Loans
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetAll()
    {
    var loans = await _loanService.GetAllLoansAsync();
    return Ok(loans);
    }

    // GET: api/Loans/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Loan>> GetById(Guid id)
    {
    var loan = await _loanService.GetLoanByIdAsync(id);

    if (loan == null)
        return NotFound(new { Message = "El préstamo solicitado no existe." });

    return Ok(loan);
    }
}

    // recibo JSON
public record SimulationRequest(decimal Amount, decimal Tea, int Term);