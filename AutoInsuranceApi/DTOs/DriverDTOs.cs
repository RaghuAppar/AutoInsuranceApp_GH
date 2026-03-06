namespace AutoInsuranceApi.DTOs;

public class DriverDto
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? LicenseNumber { get; set; }
    public string? LicenseState { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDriverRequest
{
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? LicenseNumber { get; set; }
    public string? LicenseState { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public bool IsPrimary { get; set; }
}

public class UpdateDriverRequest : CreateDriverRequest { }
