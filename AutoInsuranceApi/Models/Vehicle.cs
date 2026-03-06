namespace AutoInsuranceApi.Models;

public class Vehicle
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    public string Vin { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Usage { get; set; } // Personal, Commercial
    public int? AnnualMileage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CustomerProfile CustomerProfile { get; set; } = null!;
}
