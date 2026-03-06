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
public class ProfilesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProfilesController(AppDbContext db)
    {
        _db = db;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>GET /api/profiles/me - Get current user's profile</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var profile = await _db.CustomerProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == UserId, ct);
        if (profile == null)
            return NotFound();

        return Ok(MapToDto(profile));
    }

    /// <summary>PUT /api/profiles/me - Create or update profile</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] CreateOrUpdateProfileRequest request, CancellationToken ct)
    {
        var profile = await _db.CustomerProfiles.FirstOrDefaultAsync(p => p.UserId == UserId, ct);
        if (profile == null)
        {
            profile = new CustomerProfile { UserId = UserId };
            _db.CustomerProfiles.Add(profile);
            await _db.SaveChangesAsync(ct);
        }

        profile.DateOfBirth = request.DateOfBirth;
        profile.AddressLine1 = request.AddressLine1;
        profile.AddressLine2 = request.AddressLine2;
        profile.City = request.City;
        profile.State = request.State;
        profile.PostalCode = request.PostalCode;
        profile.LicenseNumber = request.LicenseNumber;
        profile.LicenseState = request.LicenseState;
        profile.LicenseExpiry = request.LicenseExpiry;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        _db.AuditLogs.Add(new AuditLog { UserId = UserId, Action = "ProfileUpdate", EntityType = "CustomerProfile", EntityId = profile.Id.ToString() });
        await _db.SaveChangesAsync(ct);

        return Ok(MapToDto(profile));
    }

    private static CustomerProfileDto MapToDto(CustomerProfile p) => new()
    {
        Id = p.Id,
        UserId = p.UserId,
        DateOfBirth = p.DateOfBirth,
        AddressLine1 = p.AddressLine1,
        AddressLine2 = p.AddressLine2,
        City = p.City,
        State = p.State,
        PostalCode = p.PostalCode,
        LicenseNumber = p.LicenseNumber,
        LicenseState = p.LicenseState,
        LicenseExpiry = p.LicenseExpiry,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
