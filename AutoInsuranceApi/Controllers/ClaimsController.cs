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
public class ClaimsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClaimsController(AppDbContext db)
    {
        _db = db;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>GET /api/claims - List claims for current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await _db.Claims
            .Where(c => c.UserId == UserId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ClaimDto
            {
                Id = c.Id,
                UserId = c.UserId,
                PolicyId = c.PolicyId,
                ClaimNumber = c.ClaimNumber,
                IncidentDate = c.IncidentDate,
                Description = c.Description,
                Location = c.Location,
                InvolvedParties = c.InvolvedParties,
                ClaimType = c.ClaimType,
                Status = c.Status,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(ct);
        return Ok(list);
    }

    /// <summary>GET /api/claims/{id}</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var claim = await _db.Claims
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == UserId, ct);
        if (claim == null)
            return NotFound();

        var dto = new ClaimDto
        {
            Id = claim.Id,
            UserId = claim.UserId,
            PolicyId = claim.PolicyId,
            ClaimNumber = claim.ClaimNumber,
            IncidentDate = claim.IncidentDate,
            Description = claim.Description,
            Location = claim.Location,
            InvolvedParties = claim.InvolvedParties,
            ClaimType = claim.ClaimType,
            Status = claim.Status,
            Notes = claim.Notes,
            CreatedAt = claim.CreatedAt,
            UpdatedAt = claim.UpdatedAt,
            Documents = claim.Documents.Select(d => new ClaimDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                ContentType = d.ContentType,
                DocumentType = d.DocumentType,
                UploadedAt = d.UploadedAt
            }).ToList()
        };
        return Ok(dto);
    }

    /// <summary>POST /api/claims - Submit new claim</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateClaimRequest request, CancellationToken ct)
    {
        if (request.PolicyId.HasValue)
        {
            var policyExists = await _db.Policies.AnyAsync(p => p.Id == request.PolicyId && p.UserId == UserId, ct);
            if (!policyExists)
                return BadRequest(new { message = "Policy not found." });
        }

        var claimNumber = "CLM-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
        var claim = new Models.Claim
        {
            UserId = UserId,
            PolicyId = request.PolicyId,
            ClaimNumber = claimNumber,
            IncidentDate = request.IncidentDate,
            Description = request.Description.Trim(),
            Location = request.Location,
            InvolvedParties = request.InvolvedParties,
            ClaimType = request.ClaimType ?? "FirstParty",
            Status = "Submitted"
        };
        _db.Claims.Add(claim);
        await _db.SaveChangesAsync(ct);

        _db.AuditLogs.Add(new AuditLog { UserId = UserId, Action = "ClaimSubmit", EntityType = "Claim", EntityId = claim.Id.ToString() });
        await _db.SaveChangesAsync(ct);

        var dto = new ClaimDto
        {
            Id = claim.Id,
            UserId = claim.UserId,
            PolicyId = claim.PolicyId,
            ClaimNumber = claim.ClaimNumber,
            IncidentDate = claim.IncidentDate,
            Description = claim.Description,
            Location = claim.Location,
            InvolvedParties = claim.InvolvedParties,
            ClaimType = claim.ClaimType,
            Status = claim.Status,
            CreatedAt = claim.CreatedAt,
            UpdatedAt = claim.UpdatedAt,
            Documents = new List<ClaimDocumentDto>()
        };

        return CreatedAtAction(nameof(GetById), new { id = claim.Id }, dto);
    }

    /// <summary>PUT /api/claims/{id}/status - Update claim status (e.g. Agent/Admin)</summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateClaimStatusRequest request, CancellationToken ct)
    {
        var claim = await _db.Claims.FirstOrDefaultAsync(c => c.Id == id && c.UserId == UserId, ct);
        if (claim == null)
            return NotFound();

        claim.Status = request.Status;
        claim.Notes = request.Notes ?? claim.Notes;
        claim.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        _db.AuditLogs.Add(new AuditLog { UserId = UserId, Action = "ClaimStatusUpdate", EntityType = "Claim", EntityId = claim.Id.ToString(), Details = request.Status });
        await _db.SaveChangesAsync(ct);

        var updated = await _db.Claims
            .Include(c => c.Documents)
            .FirstAsync(c => c.Id == id && c.UserId == UserId, ct);
        var dto = new ClaimDto
        {
            Id = updated.Id,
            UserId = updated.UserId,
            PolicyId = updated.PolicyId,
            ClaimNumber = updated.ClaimNumber,
            IncidentDate = updated.IncidentDate,
            Description = updated.Description,
            Location = updated.Location,
            InvolvedParties = updated.InvolvedParties,
            ClaimType = updated.ClaimType,
            Status = updated.Status,
            Notes = updated.Notes,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt,
            Documents = updated.Documents.Select(d => new ClaimDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                ContentType = d.ContentType,
                DocumentType = d.DocumentType,
                UploadedAt = d.UploadedAt
            }).ToList()
        };
        return Ok(dto);
    }
}
