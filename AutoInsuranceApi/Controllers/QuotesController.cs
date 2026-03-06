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
public class QuotesController : ControllerBase
{
    private readonly AppDbContext _db;

    public QuotesController(AppDbContext db)
    {
        _db = db;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>GET /api/quotes - List quotes for current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<QuoteSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await _db.Quotes
            .Where(q => q.UserId == UserId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuoteSummaryDto
            {
                Id = q.Id,
                QuoteNumber = q.QuoteNumber,
                TotalPremium = q.TotalPremium,
                Status = q.Status,
                ValidUntil = q.ValidUntil,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync(ct);
        return Ok(list);
    }

    /// <summary>GET /api/quotes/{id} - Get quote by id with vehicles</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var quote = await _db.Quotes
            .Include(q => q.QuoteVehicles)
            .ThenInclude(qv => qv.Vehicle)
            .FirstOrDefaultAsync(q => q.Id == id && q.UserId == UserId, ct);
        if (quote == null)
            return NotFound();

        var dto = new QuoteDto
        {
            Id = quote.Id,
            UserId = quote.UserId,
            QuoteNumber = quote.QuoteNumber,
            TotalPremium = quote.TotalPremium,
            LiabilityPremium = quote.LiabilityPremium,
            CollisionPremium = quote.CollisionPremium,
            ComprehensivePremium = quote.ComprehensivePremium,
            Deductible = quote.Deductible,
            LiabilityLimit = quote.LiabilityLimit,
            Status = quote.Status,
            ValidUntil = quote.ValidUntil,
            CreatedAt = quote.CreatedAt,
            Vehicles = quote.QuoteVehicles.Select(qv => new VehicleDto
            {
                Id = qv.Vehicle.Id,
                CustomerProfileId = qv.Vehicle.CustomerProfileId,
                Vin = qv.Vehicle.Vin,
                Make = qv.Vehicle.Make,
                Model = qv.Vehicle.Model,
                Year = qv.Vehicle.Year,
                Usage = qv.Vehicle.Usage,
                AnnualMileage = qv.Vehicle.AnnualMileage,
                CreatedAt = qv.Vehicle.CreatedAt
            }).ToList()
        };
        return Ok(dto);
    }

    /// <summary>POST /api/quotes - Create a new quote (premium calculation)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateQuoteRequest request, CancellationToken ct)
    {
        var profileId = await _db.CustomerProfiles.Where(p => p.UserId == UserId).Select(p => p.Id).FirstOrDefaultAsync(ct);
        if (profileId == 0)
            return BadRequest(new { message = "Create a profile first." });

        if (request.VehicleIds == null || !request.VehicleIds.Any())
            return BadRequest(new { message = "At least one vehicle is required." });

        var vehicles = await _db.Vehicles
            .Where(v => v.CustomerProfileId == profileId && request.VehicleIds.Contains(v.Id))
            .ToListAsync(ct);
        if (vehicles.Count != request.VehicleIds.Count)
            return BadRequest(new { message = "One or more vehicle IDs are invalid." });

        // Simple premium calculation: base + per vehicle + deductibles/limits
        decimal baseRate = 200m;
        decimal perVehicle = 150m * vehicles.Count;
        decimal liability = 80m + (request.LiabilityLimit / 10000 * 5m);
        decimal collision = request.IncludeCollision ? 120m - (request.Deductible / 100m) : 0;
        decimal comprehensive = request.IncludeComprehensive ? 60m : 0;
        decimal total = baseRate + perVehicle + liability + collision + comprehensive;

        var quoteNumber = "Q-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
        var validUntil = DateTime.UtcNow.AddDays(30);

        var quote = new Quote
        {
            UserId = UserId,
            QuoteNumber = quoteNumber,
            TotalPremium = total,
            LiabilityPremium = liability,
            CollisionPremium = collision,
            ComprehensivePremium = comprehensive,
            Deductible = request.Deductible,
            LiabilityLimit = request.LiabilityLimit,
            Status = "Completed",
            ValidUntil = validUntil
        };
        _db.Quotes.Add(quote);
        await _db.SaveChangesAsync(ct);

        foreach (var v in vehicles)
        {
            _db.QuoteVehicles.Add(new QuoteVehicle { QuoteId = quote.Id, VehicleId = v.Id });
        }
        await _db.SaveChangesAsync(ct);

        var created = await _db.Quotes
            .Include(q => q.QuoteVehicles)
            .ThenInclude(qv => qv.Vehicle)
            .FirstAsync(q => q.Id == quote.Id, ct);

        var dto = new QuoteDto
        {
            Id = created.Id,
            UserId = created.UserId,
            QuoteNumber = created.QuoteNumber,
            TotalPremium = created.TotalPremium,
            LiabilityPremium = created.LiabilityPremium,
            CollisionPremium = created.CollisionPremium,
            ComprehensivePremium = created.ComprehensivePremium,
            Deductible = created.Deductible,
            LiabilityLimit = created.LiabilityLimit,
            Status = created.Status,
            ValidUntil = created.ValidUntil,
            CreatedAt = created.CreatedAt,
            Vehicles = created.QuoteVehicles.Select(qv => new VehicleDto
            {
                Id = qv.Vehicle.Id,
                CustomerProfileId = qv.Vehicle.CustomerProfileId,
                Vin = qv.Vehicle.Vin,
                Make = qv.Vehicle.Make,
                Model = qv.Vehicle.Model,
                Year = qv.Vehicle.Year,
                Usage = qv.Vehicle.Usage,
                AnnualMileage = qv.Vehicle.AnnualMileage,
                CreatedAt = qv.Vehicle.CreatedAt
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = quote.Id }, dto);
    }
}
