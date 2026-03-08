namespace AutoInsuranceApi.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public int PolicyId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRefund { get; set; }
}

public class CreatePaymentRequest
{
    public int PolicyId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Card";
    public string? PaymentToken { get; set; }
}

/// <summary>Request for dummy payment gateway (Card, NetBanking, UPI)</summary>
public class ProcessPaymentRequest
{
    public int PolicyId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Card"; // Card, NetBanking, UPI
    // Card
    public string? CardLast4 { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCvv { get; set; }
    // NetBanking
    public string? BankCode { get; set; }
    // UPI
    public string? UpiId { get; set; }
}
