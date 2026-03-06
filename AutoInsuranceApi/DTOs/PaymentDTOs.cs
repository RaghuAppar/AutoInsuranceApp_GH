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
