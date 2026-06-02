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
    private readonly Mock<ITransactionRepo> _transactionRepo = new();
    private readonly Mock<IUserRepo> _userRepo = new();
    private readonly Mock<IStationRepo> _stationRepo = new();
    private readonly Mock<IFuelTypeRepo> _fuelTypeRepo = new();
    private readonly Mock<IStationFuelPriceRepo> _stationFuelPriceRepo = new();
    private readonly Mock<IStationEmployeeRepo> _stationEmployeeRepo = new();
    private readonly Mock<IStationPhotoRepo> _stationPhotoRepo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        var emailSender = new EmailSender(new Mock<IConfiguration>().Object);

        _controller = new TransactionController(
            _transactionRepo.Object,
            _userRepo.Object,
            _stationRepo.Object,
            _fuelTypeRepo.Object,
            _stationFuelPriceRepo.Object,
            _stationEmployeeRepo.Object,
            _stationPhotoRepo.Object,
            _mapper.Object,
            emailSender);

        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
    }

    private void LoginAsCustomer(string identityUserId)
    {
        _controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim(ClaimTypes.NameIdentifier, identityUserId)
            }, "TestAuth"))
        };
    }

    [Fact]
    public async Task GetMyTransactions_ReturnsUnauthorized_WhenNotLoggedIn()
    {
        var result = await _controller.GetMyTransactions();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetMyTransactions_ReturnsTransactions_ForLoggedInCustomer()
    {
        var user = new User { Id = 5, IdentityUserId = "identity-abc" };
        var transactions = new List<Transaction>
        {
            new() { Id = 1, CustomerId = 5, Liters = 10 },
            new() { Id = 2, CustomerId = 5, Liters = 20 }
        };
        var dtos = new List<TransactionDto>
        {
            new() { Id = 1, Liters = 10 },
            new() { Id = 2, Liters = 20 }
        };

        LoginAsCustomer("identity-abc");
        _userRepo.Setup(r => r.GetByIdentityId("identity-abc")).ReturnsAsync(user);
        _transactionRepo.Setup(r => r.GetByUser(5)).ReturnsAsync(transactions);
        _mapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions)).Returns(dtos);

        var result = await _controller.GetMyTransactions();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dtos, ok.Value);
    }
}
