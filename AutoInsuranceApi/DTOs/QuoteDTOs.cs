namespace AutoInsuranceApi.DTOs;

public class QuoteDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    public decimal LiabilityPremium { get; set; }
    public decimal CollisionPremium { get; set; }
    public decimal ComprehensivePremium { get; set; }
    public decimal Deductible { get; set; }
    public int LiabilityLimit { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ValidUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<VehicleDto>? Vehicles { get; set; }
}

public class CreateQuoteRequest
{
    public List<int> VehicleIds { get; set; } = new();
    public decimal Deductible { get; set; } = 500;
    public int LiabilityLimit { get; set; } = 100000;
    public bool IncludeCollision { get; set; } = true;
    public bool IncludeComprehensive { get; set; } = true;
}

public class QuoteSummaryDto
{
    public int Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ValidUntil { get; set; }
    public DateTime CreatedAt { get; set; }
}
