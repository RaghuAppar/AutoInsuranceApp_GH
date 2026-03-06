namespace AutoInsuranceApi.DTOs;

public class ClaimDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? PolicyId { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public DateTime IncidentDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? InvolvedParties { get; set; }
    public string ClaimType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ClaimDocumentDto>? Documents { get; set; }
}

public class ClaimDocumentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class CreateClaimRequest
{
    public int? PolicyId { get; set; }
    public DateTime IncidentDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? InvolvedParties { get; set; }
    public string ClaimType { get; set; } = "FirstParty";
}

public class UpdateClaimStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
