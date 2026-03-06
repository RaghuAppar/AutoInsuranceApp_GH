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
public class PoliciesController : ControllerBase
{
    private readonly AppDbContext _db;

    public PoliciesController(AppDbContext db)
    {
        _db = db;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>GET /api/policies - List policies for current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PolicySummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await _db.Policies
            .Where(p => p.UserId == UserId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PolicySummaryDto
            {
                Id = p.Id,
                PolicyNumber = p.PolicyNumber,
                EffectiveDate = p.EffectiveDate,
                ExpiryDate = p.ExpiryDate,
                TotalPremium = p.TotalPremium,
                Status = p.Status
            })
            .ToListAsync(ct);
        return Ok(list);
    }

    /// <summary>GET /api/policies/{id}</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var policy = await _db.Policies
            .Include(p => p.PolicyVehicles)
            .ThenInclude(pv => pv.Vehicle)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId, ct);
        if (policy == null)
            return NotFound();

        var dto = new PolicyDto
        {
            Id = policy.Id,
            UserId = policy.UserId,
            QuoteId = policy.QuoteId,
            PolicyNumber = policy.PolicyNumber,
            EffectiveDate = policy.EffectiveDate,
            ExpiryDate = policy.ExpiryDate,
            TotalPremium = policy.TotalPremium,
            PaymentPlan = policy.PaymentPlan,
            Status = policy.Status,
            CreatedAt = policy.CreatedAt,
            UpdatedAt = policy.UpdatedAt,
            Vehicles = policy.PolicyVehicles.Select(pv => new VehicleDto
            {
                Id = pv.Vehicle.Id,
                CustomerProfileId = pv.Vehicle.CustomerProfileId,
                Vin = pv.Vehicle.Vin,
                Make = pv.Vehicle.Make,
                Model = pv.Vehicle.Model,
                Year = pv.Vehicle.Year,
                Usage = pv.Vehicle.Usage,
                AnnualMileage = pv.Vehicle.AnnualMileage,
                CreatedAt = pv.Vehicle.CreatedAt
            }).ToList()
        };
        return Ok(dto);
    }

    /// <summary>POST /api/policies/purchase - Purchase policy from quote</summary>
    [HttpPost("purchase")]
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Purchase([FromBody] PurchasePolicyRequest request, CancellationToken ct)
    {
        var quote = await _db.Quotes
            .Include(q => q.QuoteVehicles)
            .ThenInclude(qv => qv.Vehicle)
            .FirstOrDefaultAsync(q => q.Id == request.QuoteId && q.UserId == UserId, ct);
        if (quote == null)
            return BadRequest(new { message = "Quote not found or expired." });
        if (quote.Status == "Converted")
            return BadRequest(new { message = "Quote already converted to policy." });
        if (quote.ValidUntil < DateTime.UtcNow)
            return BadRequest(new { message = "Quote has expired." });

        var policyNumber = "POL-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + new Random().Next(10000, 99999);
        var effective = DateTime.UtcNow.Date;
        var expiry = effective.AddYears(1);

        var policy = new Policy
        {
            UserId = UserId,
            QuoteId = quote.Id,
            PolicyNumber = policyNumber,
            EffectiveDate = effective,
            ExpiryDate = expiry,
            TotalPremium = quote.TotalPremium,
            PaymentPlan = request.PaymentPlan ?? "Full",
            Status = "Active"
        };
        _db.Policies.Add(policy);
        await _db.SaveChangesAsync(ct);

        foreach (var qv in quote.QuoteVehicles)
        {
            _db.PolicyVehicles.Add(new PolicyVehicle { PolicyId = policy.Id, VehicleId = qv.VehicleId });
        }
        quote.Status = "Converted";
        await _db.SaveChangesAsync(ct);

        if (!string.IsNullOrEmpty(request.PaymentToken))
        {
            _db.Payments.Add(new Payment
            {
                PolicyId = policy.Id,
                Amount = quote.TotalPremium,
                PaymentMethod = "Card",
                PaymentToken = request.PaymentToken,
                Status = "Completed",
                PaidAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(ct);
        }

        _db.AuditLogs.Add(new AuditLog { UserId = UserId, Action = "PolicyPurchase", EntityType = "Policy", EntityId = policy.Id.ToString() });
        await _db.SaveChangesAsync(ct);

        var created = await _db.Policies
            .Include(p => p.PolicyVehicles)
            .ThenInclude(pv => pv.Vehicle)
            .FirstAsync(p => p.Id == policy.Id, ct);

        var dto = new PolicyDto
        {
            Id = created.Id,
            UserId = created.UserId,
            QuoteId = created.QuoteId,
            PolicyNumber = created.PolicyNumber,
            EffectiveDate = created.EffectiveDate,
            ExpiryDate = created.ExpiryDate,
            TotalPremium = created.TotalPremium,
            PaymentPlan = created.PaymentPlan,
            Status = created.Status,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt,
            Vehicles = created.PolicyVehicles.Select(pv => new VehicleDto
            {
                Id = pv.Vehicle.Id,
                CustomerProfileId = pv.Vehicle.CustomerProfileId,
                Vin = pv.Vehicle.Vin,
                Make = pv.Vehicle.Make,
                Model = pv.Vehicle.Model,
                Year = pv.Vehicle.Year,
                Usage = pv.Vehicle.Usage,
                AnnualMileage = pv.Vehicle.AnnualMileage,
                CreatedAt = pv.Vehicle.CreatedAt
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = policy.Id }, dto);
    }

    /// <summary>POST /api/policies/{id}/cancel - Cancel policy</summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var policy = await _db.Policies.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId, ct);
        if (policy == null)
            return NotFound();
        if (policy.Status != "Active")
            return BadRequest(new { message = "Only active policies can be cancelled." });

        policy.Status = "Cancelled";
        policy.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        _db.AuditLogs.Add(new AuditLog { UserId = UserId, Action = "PolicyCancel", EntityType = "Policy", EntityId = policy.Id.ToString() });
        await _db.SaveChangesAsync(ct);

        var updated = await _db.Policies
            .Include(p => p.PolicyVehicles)
            .ThenInclude(pv => pv.Vehicle)
            .FirstAsync(p => p.Id == id, ct);
        var dto = new PolicyDto
        {
            Id = updated.Id,
            UserId = updated.UserId,
            QuoteId = updated.QuoteId,
            PolicyNumber = updated.PolicyNumber,
            EffectiveDate = updated.EffectiveDate,
            ExpiryDate = updated.ExpiryDate,
            TotalPremium = updated.TotalPremium,
            PaymentPlan = updated.PaymentPlan,
            Status = updated.Status,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt,
            Vehicles = updated.PolicyVehicles.Select(pv => new VehicleDto
            {
                Id = pv.Vehicle.Id,
                CustomerProfileId = pv.Vehicle.CustomerProfileId,
                Vin = pv.Vehicle.Vin,
                Make = pv.Vehicle.Make,
                Model = pv.Vehicle.Model,
                Year = pv.Vehicle.Year,
                Usage = pv.Vehicle.Usage,
                AnnualMileage = pv.Vehicle.AnnualMileage,
                CreatedAt = pv.Vehicle.CreatedAt
            }).ToList()
        };
        return Ok(dto);
    }
}
