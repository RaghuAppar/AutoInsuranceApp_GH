using AutoInsuranceApi.Data;
using AutoInsuranceApi.DTOs;
using AutoInsuranceApi.Models;
using AutoInsuranceApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace AutoInsuranceApi.Tests;

[TestFixture]
public class AuthServiceTests
{
    private DbContextOptions<AppDbContext> _options = null!;
    private Mock<IConfiguration> _configMock = null!;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["Jwt:Key"]).Returns("AutoInsuranceSecretKeyThatIsAtLeast32CharactersLong!");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("AutoInsuranceApi");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("AutoInsuranceClient");
    }

    [Test]
    public async Task RegisterAsync_WhenEmailNotTaken_ReturnsAuthResponseAndCreatesUser()
    {
        await using var db = new AppDbContext(_options);
        var service = new AuthService(db, _configMock.Object);
        var request = new RegisterRequest { Email = "new@test.com", Password = "Pass123!", FullName = "New User" };

        var result = await service.RegisterAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo(request.Email));
        Assert.That(result.FullName, Is.EqualTo(request.FullName));
        Assert.That(result.Role, Is.EqualTo("Customer"));
        Assert.That(result.Token, Is.Not.Empty);
        Assert.That(result.UserId, Is.GreaterThan(0));

        await using var db2 = new AppDbContext(_options);
        var user = await db2.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        Assert.That(user, Is.Not.Null);
        Assert.That(BCrypt.Net.BCrypt.Verify(request.Password, user!.PasswordHash), Is.True);
        var profile = await db2.CustomerProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        Assert.That(profile, Is.Not.Null);
    }

    [Test]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsNull()
    {
        await using var db = new AppDbContext(_options);
        db.Users.Add(new User { Email = "existing@test.com", PasswordHash = "hash", FullName = "Existing", Role = "Customer" });
        await db.SaveChangesAsync();

        var service = new AuthService(db, _configMock.Object);
        var request = new RegisterRequest { Email = "existing@test.com", Password = "Pass123!", FullName = "Another" };

        var result = await service.RegisterAsync(request);

        Assert.That(result, Is.Null);
        var count = await db.Users.CountAsync(u => u.Email == request.Email);
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task LoginAsync_WhenValidCredentials_ReturnsAuthResponse()
    {
        await using var db = new AppDbContext(_options);
        var hash = BCrypt.Net.BCrypt.HashPassword("Pass123!");
        db.Users.Add(new User { Email = "login@test.com", PasswordHash = hash, FullName = "Login User", Role = "Customer" });
        await db.SaveChangesAsync();

        var service = new AuthService(db, _configMock.Object);
        var request = new LoginRequest { Email = "login@test.com", Password = "Pass123!" };

        var result = await service.LoginAsync(request, "127.0.0.1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo(request.Email));
        Assert.That(result.Token, Is.Not.Empty);
    }

    [Test]
    public async Task LoginAsync_WhenWrongPassword_ReturnsNull()
    {
        await using var db = new AppDbContext(_options);
        db.Users.Add(new User { Email = "u@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct"), FullName = "User", Role = "Customer" });
        await db.SaveChangesAsync();

        var service = new AuthService(db, _configMock.Object);
        var result = await service.LoginAsync(new LoginRequest { Email = "u@test.com", Password = "Wrong" }, null);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task LoginAsync_WhenUserNotFound_ReturnsNull()
    {
        await using var db = new AppDbContext(_options);
        var service = new AuthService(db, _configMock.Object);
        var result = await service.LoginAsync(new LoginRequest { Email = "nonexistent@test.com", Password = "any" }, null);
        Assert.That(result, Is.Null);
    }
}
