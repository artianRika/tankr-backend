using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TankR.Controllers;
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
