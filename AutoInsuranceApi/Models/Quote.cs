namespace AutoInsuranceApi.Models;

public class Quote
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    public decimal LiabilityPremium { get; set; }
    public decimal CollisionPremium { get; set; }
    public decimal ComprehensivePremium { get; set; }
    public decimal Deductible { get; set; }
    public int LiabilityLimit { get; set; } = 100000;
    public string Status { get; set; } = "Draft"; // Draft, Completed, Expired, Converted
    public DateTime ValidUntil { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<QuoteVehicle> QuoteVehicles { get; set; } = new List<QuoteVehicle>();
}

public class QuoteVehicle
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public int VehicleId { get; set; }
    public Quote Quote { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
