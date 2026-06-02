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
    private readonly Mock<IStationRepo> _stationRepo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly StationController _controller;

    public StationControllerTests()
    {
        _controller = new StationController(
            _stationRepo.Object,
            _mapper.Object,
            new Mock<IFreeImageService>().Object);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenStationMissing()
    {
        _stationRepo.Setup(r => r.GetById(999)).ReturnsAsync((Station?)null);

        var result = await _controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_ReturnsAllStations()
    {
        var stations = new List<Station>
        {
            new() { Id = 1, Name = "TankR North" },
            new() { Id = 2, Name = "TankR South" }
        };
        var dtos = new List<StationDto>
        {
            new() { Id = 1, Name = "TankR North" },
            new() { Id = 2, Name = "TankR South" }
        };

        _stationRepo.Setup(r => r.GetAll()).ReturnsAsync(stations);
        _mapper.Setup(m => m.Map<IEnumerable<StationDto>>(stations)).Returns(dtos);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(2, Assert.IsAssignableFrom<IEnumerable<StationDto>>(ok.Value).Count());
    }
}
