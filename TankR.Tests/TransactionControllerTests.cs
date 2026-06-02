using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TankR.Controllers;
using TankR.Data.Dtos.Transactions;
using TankR.Data.Models;
using TankR.Repos.Interfaces;
using Xunit;

namespace TankR.Tests;

public class TransactionControllerTests
{
    private readonly Mock<ITransactionRepo> _transactionRepoMock;
    private readonly Mock<IUserRepo> _userRepoMock;
    private readonly Mock<IStationRepo> _stationRepoMock;
    private readonly Mock<IFuelTypeRepo> _fuelTypeRepoMock;
    private readonly Mock<IStationFuelPriceRepo> _stationFuelPriceRepoMock;
    private readonly Mock<IStationEmployeeRepo> _stationEmployeeRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        _transactionRepoMock = new Mock<ITransactionRepo>();
        _userRepoMock = new Mock<IUserRepo>();
        _stationRepoMock = new Mock<IStationRepo>();
        _fuelTypeRepoMock = new Mock<IFuelTypeRepo>();
        _stationFuelPriceRepoMock = new Mock<IStationFuelPriceRepo>();
        _stationEmployeeRepoMock = new Mock<IStationEmployeeRepo>();
        _mapperMock = new Mock<IMapper>();

        var configMock = new Mock<IConfiguration>();
        var emailSender = new EmailSender(configMock.Object);

        _controller = new TransactionController(
            _transactionRepoMock.Object,
            _userRepoMock.Object,
            _stationRepoMock.Object,
            _fuelTypeRepoMock.Object,
            _stationFuelPriceRepoMock.Object,
            _stationEmployeeRepoMock.Object,
            _mapperMock.Object,
            emailSender);

        // Provide a minimal HttpContext so User claims don't throw
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private void SetCustomerIdentity(string? identityUserId)
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, "Customer") };
        if (!string.IsNullOrEmpty(identityUserId))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUserId));

        _controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
        };
    }

    [Fact]
    public async Task GetMyTransactions_ReturnsUnauthorized_WhenIdentityClaimMissing()
    {
        SetCustomerIdentity(null);

        var result = await _controller.GetMyTransactions();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetMyTransactions_ReturnsNotFound_WhenDomainUserDoesNotExist()
    {
        SetCustomerIdentity("identity-abc");
        _userRepoMock.Setup(r => r.GetByIdentityId("identity-abc")).ReturnsAsync((User?)null);

        var result = await _controller.GetMyTransactions();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetMyTransactions_ReturnsOk_WithMappedDtos_WhenUserHasTransactions()
    {
        SetCustomerIdentity("identity-abc");
        var domainUser = new User { Id = 5, IdentityUserId = "identity-abc" };
        var transactions = new List<Transaction>
        {
            new() { Id = 1, CustomerId = 5, StationId = 1, FuelTypeId = 1, Liters = 10 },
            new() { Id = 2, CustomerId = 5, StationId = 1, FuelTypeId = 2, Liters = 20 }
        };
        var dtos = new List<TransactionDto>
        {
            new() { Id = 1, Liters = 10 },
            new() { Id = 2, Liters = 20 }
        };

        _userRepoMock.Setup(r => r.GetByIdentityId("identity-abc")).ReturnsAsync(domainUser);
        _transactionRepoMock.Setup(r => r.GetByUser(5)).ReturnsAsync(transactions);
        _mapperMock
            .Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(dtos);

        var result = await _controller.GetMyTransactions();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dtos, okResult.Value);
        _transactionRepoMock.Verify(r => r.GetByUser(5), Times.Once);
    }

    [Fact]
    public async Task GetMyTransactions_ReturnsOk_WithEmptyList_WhenUserHasNoTransactions()
    {
        SetCustomerIdentity("identity-abc");
        var domainUser = new User { Id = 5, IdentityUserId = "identity-abc" };

        _userRepoMock.Setup(r => r.GetByIdentityId("identity-abc")).ReturnsAsync(domainUser);
        _transactionRepoMock.Setup(r => r.GetByUser(5)).ReturnsAsync((IEnumerable<Transaction>?)null);
        _mapperMock
            .Setup(m => m.Map<IEnumerable<TransactionDto>>(It.IsAny<IEnumerable<Transaction>>()))
            .Returns(Enumerable.Empty<TransactionDto>());

        var result = await _controller.GetMyTransactions();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Empty((IEnumerable<TransactionDto>)okResult.Value!);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        _transactionRepoMock.Setup(r => r.GetById(42)).ReturnsAsync((Transaction?)null);

        var result = await _controller.GetById(42);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByStation_ReturnsNotFound_WhenStationDoesNotExist()
    {
        _stationRepoMock.Setup(r => r.GetById(55)).ReturnsAsync((Station?)null);

        var result = await _controller.GetByStation(55);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
