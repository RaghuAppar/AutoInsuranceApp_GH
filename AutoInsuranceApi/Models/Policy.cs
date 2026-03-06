namespace AutoInsuranceApi.Models;

public class Policy
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? QuoteId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal TotalPremium { get; set; }
    public string PaymentPlan { get; set; } = "Full"; // Full, Monthly
    public string Status { get; set; } = "Active"; // Active, Cancelled, Expired, Pending
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Quote? Quote { get; set; }
    public ICollection<PolicyVehicle> PolicyVehicles { get; set; } = new List<PolicyVehicle>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class PolicyVehicle
{
    public int Id { get; set; }
    public int PolicyId { get; set; }
    public int VehicleId { get; set; }
    public Policy Policy { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
