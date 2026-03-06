using System.Security.Claims;
using AutoInsuranceApi.Controllers;
using AutoInsuranceApi.Data;
using AutoInsuranceApi.DTOs;
using AutoInsuranceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AutoInsuranceApi.Tests;

[TestFixture]
public class QuotesControllerTests
{
    private DbContextOptions<AppDbContext> _options = null!;
    private const int TestUserId = 42;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private static void SetUser(QuotesController controller, int userId)
    {
        var identity = new ClaimsIdentity(new[] {
            new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.Email, "u@test.com"),
            new System.Security.Claims.Claim(ClaimTypes.Role, "Customer")
        }, "Test");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task GetAll_ReturnsOnlyCurrentUserQuotes()
    {
        await using var db = new AppDbContext(_options);
        db.Users.Add(new User { Id = TestUserId, Email = "u@test.com", PasswordHash = "h", FullName = "U", Role = "Customer" });
        db.Users.Add(new User { Id = TestUserId + 1, Email = "other@test.com", PasswordHash = "h", FullName = "O", Role = "Customer" });
        db.Quotes.Add(new Quote { UserId = TestUserId, QuoteNumber = "Q-1", TotalPremium = 100, Status = "Completed", ValidUntil = DateTime.UtcNow.AddDays(30) });
        db.Quotes.Add(new Quote { UserId = TestUserId + 1, QuoteNumber = "Q-2", TotalPremium = 200, Status = "Completed", ValidUntil = DateTime.UtcNow.AddDays(30) });
        await db.SaveChangesAsync();

        var controller = new QuotesController(db);
        SetUser(controller, TestUserId);

        var result = await controller.GetAll(CancellationToken.None);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var list = ok!.Value as List<QuoteSummaryDto>;
        Assert.That(list, Is.Not.Null);
        Assert.That(list!.Count, Is.EqualTo(1));
        Assert.That(list[0].QuoteNumber, Is.EqualTo("Q-1"));
    }

    [Test]
    public async Task GetById_WhenQuoteBelongsToUser_ReturnsQuote()
    {
        await using var db = new AppDbContext(_options);
        db.Users.Add(new User { Id = TestUserId, Email = "u@test.com", PasswordHash = "h", FullName = "U", Role = "Customer" });
        db.CustomerProfiles.Add(new CustomerProfile { UserId = TestUserId });
        await db.SaveChangesAsync();
        var profileId = await db.CustomerProfiles.Where(p => p.UserId == TestUserId).Select(p => p.Id).FirstAsync();
        db.Vehicles.Add(new Vehicle { CustomerProfileId = profileId, Vin = "VIN", Make = "M", Model = "M", Year = 2020 });
        await db.SaveChangesAsync();
        var vehicleId = await db.Vehicles.Where(v => v.CustomerProfileId == profileId).Select(v => v.Id).FirstAsync();
        db.Quotes.Add(new Quote { UserId = TestUserId, QuoteNumber = "Q-1", TotalPremium = 100, Status = "Completed", ValidUntil = DateTime.UtcNow.AddDays(30) });
        await db.SaveChangesAsync();
        var quoteId = await db.Quotes.Where(q => q.UserId == TestUserId).Select(q => q.Id).FirstAsync();
        db.QuoteVehicles.Add(new QuoteVehicle { QuoteId = quoteId, VehicleId = vehicleId });
        await db.SaveChangesAsync();

        var controller = new QuotesController(db);
        SetUser(controller, TestUserId);

        var result = await controller.GetById(quoteId, CancellationToken.None);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as QuoteDto;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Id, Is.EqualTo(quoteId));
        Assert.That(dto.QuoteNumber, Is.EqualTo("Q-1"));
        Assert.That(dto.Vehicles, Is.Not.Null);
        Assert.That(dto.Vehicles!.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetById_WhenQuoteNotFoundOrOtherUser_ReturnsNotFound()
    {
        await using var db = new AppDbContext(_options);
        db.Users.Add(new User { Id = TestUserId, Email = "u@test.com", PasswordHash = "h", FullName = "U", Role = "Customer" });
        db.Quotes.Add(new Quote { UserId = TestUserId + 1, QuoteNumber = "Q-other", TotalPremium = 100, Status = "Completed", ValidUntil = DateTime.UtcNow.AddDays(30) });
        await db.SaveChangesAsync();
        var otherQuoteId = await db.Quotes.Where(q => q.UserId == TestUserId + 1).Select(q => q.Id).FirstAsync();

        var controller = new QuotesController(db);
        SetUser(controller, TestUserId);

        var result = await controller.GetById(otherQuoteId, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_WhenNoProfile_ReturnsBadRequest()
    {
        await using var db = new AppDbContext(_options);
        db.Users.Add(new User { Id = TestUserId, Email = "u@test.com", PasswordHash = "h", FullName = "U", Role = "Customer" });
        await db.SaveChangesAsync();

        var controller = new QuotesController(db);
        SetUser(controller, TestUserId);
        var request = new CreateQuoteRequest { VehicleIds = new List<int> { 1 }, LiabilityLimit = 100000, Deductible = 500 };

        var result = await controller.Create(request, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
    }

    [Test]
    public async Task Create_WhenValidRequest_CreatesQuoteAndReturnsCreated()
    {
        await using var db = new AppDbContext(_options);
        db.Users.Add(new User { Id = TestUserId, Email = "u@test.com", PasswordHash = "h", FullName = "U", Role = "Customer" });
        db.CustomerProfiles.Add(new CustomerProfile { UserId = TestUserId });
        await db.SaveChangesAsync();
        var profileId = await db.CustomerProfiles.Where(p => p.UserId == TestUserId).Select(p => p.Id).FirstAsync();
        db.Vehicles.Add(new Vehicle { CustomerProfileId = profileId, Vin = "VIN", Make = "M", Model = "M", Year = 2020 });
        await db.SaveChangesAsync();
        var vehicleId = await db.Vehicles.Where(v => v.CustomerProfileId == profileId).Select(v => v.Id).FirstAsync();

        var controller = new QuotesController(db);
        SetUser(controller, TestUserId);
        var request = new CreateQuoteRequest { VehicleIds = new List<int> { vehicleId }, LiabilityLimit = 100000, Deductible = 500, IncludeCollision = true, IncludeComprehensive = true };

        var result = await controller.Create(request, CancellationToken.None);

        var created = result as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);
        var dto = created!.Value as QuoteDto;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.TotalPremium, Is.GreaterThan(0));
        Assert.That(dto.Status, Is.EqualTo("Completed"));

        await using var db2 = new AppDbContext(_options);
        var quoteCount = await db2.Quotes.CountAsync(q => q.UserId == TestUserId);
        Assert.That(quoteCount, Is.EqualTo(1));
    }
}
