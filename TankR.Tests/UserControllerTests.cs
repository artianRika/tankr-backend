using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TankR.Controllers;
using TankR.Data.Dtos;
using TankR.Data.Enums;
using TankR.Data.Models;
using TankR.Repos.Interfaces;
using Xunit;

namespace TankR.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserRepo> _userRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userRepoMock = new Mock<IUserRepo>();
        _mapperMock = new Mock<IMapper>();
        _controller = new UserController(_userRepoMock.Object, _mapperMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private void SetIdentityUserId(string? identityUserId)
    {
        var claims = string.IsNullOrEmpty(identityUserId)
            ? Array.Empty<Claim>()
            : new[] { new Claim(ClaimTypes.NameIdentifier, identityUserId) };
        _controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims))
        };
    }

    [Fact]
    public async Task GetMe_ReturnsUnauthorized_WhenIdentityClaimMissing()
    {
        SetIdentityUserId(null);

        var result = await _controller.GetMe();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetMe_ReturnsNotFound_WhenDomainUserDoesNotExist()
    {
        SetIdentityUserId("identity-abc");
        _userRepoMock.Setup(r => r.GetByIdentityId("identity-abc")).ReturnsAsync((User?)null);

        var result = await _controller.GetMe();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WithMappedDto_WhenUserExists()
    {
        SetIdentityUserId("identity-abc");
        var user = new User
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Role = UserRole.Customer
        };
        var detailsDto = new UserDetailsDto
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Role = UserRole.Customer
        };

        _userRepoMock.Setup(r => r.GetByIdentityId("identity-abc")).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDetailsDto>(user)).Returns(detailsDto);

        var result = await _controller.GetMe();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(detailsDto, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _userRepoMock.Setup(r => r.GetById(99)).ReturnsAsync((User?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithMappedDto_WhenUserExists()
    {
        var user = new User { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", PhoneNumber = "070123456" };
        var userDto = new UserDto { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" };

        _userRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(userDto, okResult.Value);
    }

    [Fact]
    public async Task GetByEmail_ReturnsNotFound_WhenEmailDoesNotExist()
    {
        _userRepoMock.Setup(r => r.GetByEmail("ghost@example.com")).ReturnsAsync((User?)null);

        var result = await _controller.GetByEmail("ghost@example.com");

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}
