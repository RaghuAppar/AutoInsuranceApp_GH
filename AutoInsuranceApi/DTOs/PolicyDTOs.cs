namespace AutoInsuranceApi.DTOs;

public class PolicyDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? QuoteId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal TotalPremium { get; set; }
    public string PaymentPlan { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<VehicleDto>? Vehicles { get; set; }
}

public class PurchasePolicyRequest
{
    public int QuoteId { get; set; }
    public string PaymentPlan { get; set; } = "Full"; // Full, Monthly
    public string? PaymentToken { get; set; } // tokenized payment method
}

public class PolicySummaryDto
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal TotalPremium { get; set; }
    public string Status { get; set; } = string.Empty;
}
