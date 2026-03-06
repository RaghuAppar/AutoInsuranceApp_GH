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
public class DriversController : ControllerBase
{
    private readonly AppDbContext _db;

    public DriversController(AppDbContext db)
    {
        _db = db;
    }

    private async Task<int?> GetProfileId(CancellationToken ct) =>
        await _db.CustomerProfiles.Where(p => p.UserId == UserId).Select(p => p.Id).FirstOrDefaultAsync(ct);

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>GET /api/drivers - List drivers for current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<DriverDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        if (profileId == 0)
            return Ok(new List<DriverDto>());

        var list = await _db.Drivers
            .Where(d => d.CustomerProfileId == profileId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => MapToDto(d))
            .ToListAsync(ct);
        return Ok(list);
    }

    /// <summary>GET /api/drivers/{id}</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DriverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.Id == id && d.CustomerProfileId == profileId, ct);
        if (driver == null)
            return NotFound();
        return Ok(MapToDto(driver));
    }

    /// <summary>POST /api/drivers - Add driver</summary>
    [HttpPost]
    [ProducesResponseType(typeof(DriverDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateDriverRequest request, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        if (profileId == 0)
        {
            var profile = new CustomerProfile { UserId = UserId };
            _db.CustomerProfiles.Add(profile);
            await _db.SaveChangesAsync(ct);
            profileId = profile.Id;
        }

        var driver = new Driver
        {
            CustomerProfileId = profileId!.Value,
            FullName = request.FullName.Trim(),
            DateOfBirth = request.DateOfBirth,
            LicenseNumber = request.LicenseNumber,
            LicenseState = request.LicenseState,
            LicenseExpiry = request.LicenseExpiry,
            IsPrimary = request.IsPrimary
        };
        _db.Drivers.Add(driver);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = driver.Id }, MapToDto(driver));
    }

    /// <summary>PUT /api/drivers/{id}</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DriverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverRequest request, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.Id == id && d.CustomerProfileId == profileId, ct);
        if (driver == null)
            return NotFound();

        driver.FullName = request.FullName.Trim();
        driver.DateOfBirth = request.DateOfBirth;
        driver.LicenseNumber = request.LicenseNumber;
        driver.LicenseState = request.LicenseState;
        driver.LicenseExpiry = request.LicenseExpiry;
        driver.IsPrimary = request.IsPrimary;
        await _db.SaveChangesAsync(ct);
        return Ok(MapToDto(driver));
    }

    /// <summary>DELETE /api/drivers/{id}</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.Id == id && d.CustomerProfileId == profileId, ct);
        if (driver == null)
            return NotFound();
        _db.Drivers.Remove(driver);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static DriverDto MapToDto(Driver d) => new()
    {
        Id = d.Id,
        CustomerProfileId = d.CustomerProfileId,
        FullName = d.FullName,
        DateOfBirth = d.DateOfBirth,
        LicenseNumber = d.LicenseNumber,
        LicenseState = d.LicenseState,
        LicenseExpiry = d.LicenseExpiry,
        IsPrimary = d.IsPrimary,
        CreatedAt = d.CreatedAt
    };
}
