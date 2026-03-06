namespace AutoInsuranceApi.Models;

public class Payment
{
    public int Id { get; set; }
    public int PolicyId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Card"; // Card, ACH, Other
    public string? PaymentToken { get; set; } // tokenized, no raw card
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
    public string? FailureReason { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRefund { get; set; }

    public Policy Policy { get; set; } = null!;
}
