using Microsoft.EntityFrameworkCore;
using AutoInsuranceApi.Models;

namespace AutoInsuranceApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteVehicle> QuoteVehicles => Set<QuoteVehicle>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<PolicyVehicle> PolicyVehicles => Set<PolicyVehicle>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimDocument> ClaimDocuments => Set<ClaimDocument>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
        });
        modelBuilder.Entity<CustomerProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<CustomerProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<QuoteVehicle>()
            .HasOne(qv => qv.Quote)
            .WithMany(q => q.QuoteVehicles)
            .HasForeignKey(qv => qv.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<QuoteVehicle>()
            .HasOne(qv => qv.Vehicle)
            .WithMany()
            .HasForeignKey(qv => qv.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PolicyVehicle>()
            .HasOne(pv => pv.Policy)
            .WithMany(p => p.PolicyVehicles)
            .HasForeignKey(pv => pv.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PolicyVehicle>()
            .HasOne(pv => pv.Vehicle)
            .WithMany()
            .HasForeignKey(pv => pv.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
