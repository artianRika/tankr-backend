using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankR.Auth;
using TankR.Data;
using TankR.Data.Dtos.Identity;
using TankR.Data.Models;
using TankR.Data.Models.Identity;
using TankR.Repos.Interfaces;

namespace TankR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepo _userRepo;
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;


        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserRepo userRepo,
            AppDbContext context,
            TokenService tokenService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepo = userRepo;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
            {
                return BadRequest("Email already exists");
            }

            var identityUser = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            await _userManager.AddToRoleAsync(
                identityUser,
                dto.Role.ToString()
            );

            var user = new User
            {
                IdentityUserId = identityUser.Id,
                IdentityUser = identityUser,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
            };

            await _userRepo.Add(user);

            return Ok(new
            {
                UserId = user.Id,
                IdentityUserId = identityUser.Id
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var managedUser = await _userManager.FindByEmailAsync(dto.Email);
            if(managedUser == null)
                return BadRequest("No user found");
            
            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, dto.Password);
            if(!isPasswordValid)
                return BadRequest("Invalid password");
            
            var userInDb = _userManager.Users.FirstOrDefault(u => u.Email == dto.Email);
            if(userInDb == null)
                return Unauthorized();
            
            var accessToken =  await _tokenService.CreateToken(userInDb);
            await _context.SaveChangesAsync();

            return Ok(
                new LoginResponseDto()
                {
                    Token = accessToken,
                    Email = userInDb.Email,
                });
        }
    }

}