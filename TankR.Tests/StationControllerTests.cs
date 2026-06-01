using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TankR.Controllers;
using TankR.Data.Dtos.Stations;
using TankR.Data.Models;
using TankR.Repos.Interfaces;
using TankR.Services.Interfaces;
using Xunit;

namespace TankR.Tests;

public class StationControllerTests
{
    private readonly Mock<IStationRepo> _stationRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFreeImageService> _freeImageServiceMock;
    private readonly StationController _controller;

    public StationControllerTests()
    {
        _stationRepoMock = new Mock<IStationRepo>();
        _mapperMock = new Mock<IMapper>();
        _freeImageServiceMock = new Mock<IFreeImageService>();
        _controller = new StationController(
            _stationRepoMock.Object,
            _mapperMock.Object,
            _freeImageServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenStationDoesNotExist()
    {
        _stationRepoMock.Setup(r => r.GetById(999)).ReturnsAsync((Station?)null);

        var result = await _controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithAllStations()
    {
        var stations = new List<Station>
        {
            new Station { Id = 1, Name = "TankR North" },
            new Station { Id = 2, Name = "TankR South" }
        };
        var stationDtos = new List<StationDto>
        {
            new StationDto { Id = 1, Name = "TankR North" },
            new StationDto { Id = 2, Name = "TankR South" }
        };

        _stationRepoMock.Setup(r => r.GetAll()).ReturnsAsync(stations);
        _mapperMock.Setup(m => m.Map<IEnumerable<StationDto>>(stations)).Returns(stationDtos);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<StationDto>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }
}
