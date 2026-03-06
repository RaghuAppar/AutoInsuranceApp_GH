namespace AutoInsuranceApi.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    public string Vin { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Usage { get; set; }
    public int? AnnualMileage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleRequest
{
    public string Vin { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Usage { get; set; }
    public int? AnnualMileage { get; set; }
}

public class UpdateVehicleRequest : CreateVehicleRequest { }
