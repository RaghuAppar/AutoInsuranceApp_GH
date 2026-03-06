namespace AutoInsuranceApi.Models;

public class Claim
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? PolicyId { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public DateTime IncidentDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? InvolvedParties { get; set; }
    public string ClaimType { get; set; } = "FirstParty"; // FirstParty, ThirdParty
    public string Status { get; set; } = "Submitted"; // Submitted, UnderReview, Approved, Denied, Paid
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Policy? Policy { get; set; }
    public ICollection<ClaimDocument> Documents { get; set; } = new List<ClaimDocument>();
}

public class ClaimDocument
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StoredPath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = "Photo"; // Photo, Estimate, PoliceReport, Other
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Claim Claim { get; set; } = null!;
}
