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
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _db;

    public VehiclesController(AppDbContext db)
    {
        _db = db;
    }

    private async Task<int?> GetProfileId(CancellationToken ct) =>
        await _db.CustomerProfiles.Where(p => p.UserId == UserId).Select(p => p.Id).FirstOrDefaultAsync(ct);

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>GET /api/vehicles - List vehicles for current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        if (profileId == 0)
            return Ok(new List<VehicleDto>());

        var list = await _db.Vehicles
            .Where(v => v.CustomerProfileId == profileId)
            .OrderByDescending(v => v.CreatedAt)
            .Select(v => MapToDto(v))
            .ToListAsync(ct);
        return Ok(list);
    }

    /// <summary>GET /api/vehicles/{id} - Get vehicle by id</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == id && v.CustomerProfileId == profileId, ct);
        if (vehicle == null)
            return NotFound();
        return Ok(MapToDto(vehicle));
    }

    /// <summary>POST /api/vehicles - Add vehicle</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        if (profileId == 0)
        {
            var profile = new CustomerProfile { UserId = UserId };
            _db.CustomerProfiles.Add(profile);
            await _db.SaveChangesAsync(ct);
            profileId = profile.Id;
        }

        var vehicle = new Vehicle
        {
            CustomerProfileId = profileId!.Value,
            Vin = request.Vin.Trim(),
            Make = request.Make.Trim(),
            Model = request.Model.Trim(),
            Year = request.Year,
            Usage = request.Usage,
            AnnualMileage = request.AnnualMileage
        };
        _db.Vehicles.Add(vehicle);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, MapToDto(vehicle));
    }

    /// <summary>PUT /api/vehicles/{id} - Update vehicle</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleRequest request, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == id && v.CustomerProfileId == profileId, ct);
        if (vehicle == null)
            return NotFound();

        vehicle.Vin = request.Vin.Trim();
        vehicle.Make = request.Make.Trim();
        vehicle.Model = request.Model.Trim();
        vehicle.Year = request.Year;
        vehicle.Usage = request.Usage;
        vehicle.AnnualMileage = request.AnnualMileage;
        await _db.SaveChangesAsync(ct);
        return Ok(MapToDto(vehicle));
    }

    /// <summary>DELETE /api/vehicles/{id} - Delete vehicle</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var profileId = await GetProfileId(ct);
        var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == id && v.CustomerProfileId == profileId, ct);
        if (vehicle == null)
            return NotFound();
        _db.Vehicles.Remove(vehicle);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static VehicleDto MapToDto(Vehicle v) => new()
    {
        Id = v.Id,
        CustomerProfileId = v.CustomerProfileId,
        Vin = v.Vin,
        Make = v.Make,
        Model = v.Model,
        Year = v.Year,
        Usage = v.Usage,
        AnnualMileage = v.AnnualMileage,
        CreatedAt = v.CreatedAt
    };
}
