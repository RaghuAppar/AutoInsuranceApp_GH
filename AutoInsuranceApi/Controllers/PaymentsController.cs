using System.Security.Claims;
using AutoInsuranceApi.Data;
using AutoInsuranceApi.DTOs;
using AutoInsuranceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoInsuranceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PaymentsController(AppDbContext db)
    {
        _db = db;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>GET /api/payments - List payments for current user's policies</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? policyId, CancellationToken ct)
    {
        var query = _db.Payments
            .Include(p => p.Policy)
            .Where(p => p.Policy.UserId == UserId);

        if (policyId.HasValue)
            query = query.Where(p => p.PolicyId == policyId.Value);

        var list = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                PolicyId = p.PolicyId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                FailureReason = p.FailureReason,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt,
                IsRefund = p.IsRefund
            })
            .ToListAsync(ct);
        return Ok(list);
    }

    /// <summary>GET /api/payments/{id}</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var payment = await _db.Payments
            .Include(p => p.Policy)
            .FirstOrDefaultAsync(p => p.Id == id && p.Policy.UserId == UserId, ct);
        if (payment == null)
            return NotFound();

        return Ok(new PaymentDto
        {
            Id = payment.Id,
            PolicyId = payment.PolicyId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            FailureReason = payment.FailureReason,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt,
            IsRefund = payment.IsRefund
        });
    }

    /// <summary>POST /api/payments - Record a payment (tokenized)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request, CancellationToken ct)
    {
        var policy = await _db.Policies.FirstOrDefaultAsync(p => p.Id == request.PolicyId && p.UserId == UserId, ct);
        if (policy == null)
            return BadRequest(new { message = "Policy not found." });
        if (policy.Status != "Active")
            return BadRequest(new { message = "Policy is not active." });

        var payment = new Models.Payment
        {
            PolicyId = request.PolicyId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod ?? "Card",
            PaymentToken = request.PaymentToken,
            Status = "Completed", // In production: call payment provider then set status
            PaidAt = DateTime.UtcNow,
            IsRefund = false
        };
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);

        _db.AuditLogs.Add(new AuditLog { UserId = UserId, Action = "Payment", EntityType = "Payment", EntityId = payment.Id.ToString() });
        await _db.SaveChangesAsync(ct);

        var dto = new PaymentDto
        {
            Id = payment.Id,
            PolicyId = payment.PolicyId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt,
            IsRefund = payment.IsRefund
        };

        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, dto);
    }
}
