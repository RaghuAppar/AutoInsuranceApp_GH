namespace AutoInsuranceApi.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = "Customer"; // Customer, Agent, Admin
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public CustomerProfile? Profile { get; set; }
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}
