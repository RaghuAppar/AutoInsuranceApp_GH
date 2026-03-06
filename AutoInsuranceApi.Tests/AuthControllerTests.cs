using AutoInsuranceApi.Controllers;
using AutoInsuranceApi.DTOs;
using AutoInsuranceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace AutoInsuranceApi.Tests;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
    }

    [Test]
    public async Task Register_WhenEmailPasswordFullNameMissing_ReturnsBadRequest()
    {
        var request = new RegisterRequest { Email = "", Password = "pass", FullName = "Name" };
        var result = await _controller.Register(request, CancellationToken.None);
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        _authServiceMock.Verify(s => s.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Register_WhenValidRequest_ReturnsOkWithAuthResponse()
    {
        var request = new RegisterRequest { Email = "a@b.com", Password = "Pass123!", FullName = "Test User" };
        var authResponse = new AuthResponse { Token = "jwt", Email = request.Email, FullName = request.FullName, Role = "Customer", UserId = 1, ExpiresAt = DateTime.UtcNow.AddHours(8) };
        _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(authResponse);

        var result = await _controller.Register(request, CancellationToken.None);

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.SameAs(authResponse));
        _authServiceMock.Verify(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Register_WhenEmailAlreadyRegistered_ReturnsBadRequest()
    {
        var request = new RegisterRequest { Email = "existing@b.com", Password = "Pass123!", FullName = "User" };
        _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((AuthResponse?)null);

        var result = await _controller.Register(request, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
    }

    [Test]
    public async Task Login_WhenInvalidCredentials_ReturnsUnauthorized()
    {
        var request = new LoginRequest { Email = "a@b.com", Password = "wrong" };
        _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>())).ReturnsAsync((AuthResponse?)null);

        var result = await _controller.Login(request, CancellationToken.None);

        var unauthorized = result as UnauthorizedObjectResult;
        Assert.That(unauthorized, Is.Not.Null);
    }

    [Test]
    public async Task Login_WhenValidCredentials_ReturnsOkWithAuthResponse()
    {
        var request = new LoginRequest { Email = "a@b.com", Password = "Pass123!" };
        var authResponse = new AuthResponse { Token = "jwt", Email = request.Email, FullName = "User", Role = "Customer", UserId = 1, ExpiresAt = DateTime.UtcNow.AddHours(8) };
        _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>())).ReturnsAsync(authResponse);

        var result = await _controller.Login(request, CancellationToken.None);

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.SameAs(authResponse));
    }
}
