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
    private readonly Mock<IUserRepo> _userRepo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _controller = new UserController(_userRepo.Object, _mapper.Object);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
    }

    [Fact]
    public async Task GetMe_ReturnsUnauthorized_WhenNotLoggedIn()
    {
        var result = await _controller.GetMe();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetMe_ReturnsUser_WhenLoggedIn()
    {
        var user = new User { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", Role = UserRole.Customer };
        var dto = new UserDetailsDto { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", Role = UserRole.Customer };

        _controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "identity-abc")
            }))
        };
        _userRepo.Setup(r => r.GetByIdentityId("identity-abc")).ReturnsAsync(user);
        _mapper.Setup(m => m.Map<UserDetailsDto>(user)).Returns(dto);

        var result = await _controller.GetMe();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }
}
