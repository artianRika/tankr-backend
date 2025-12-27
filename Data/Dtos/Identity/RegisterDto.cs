using TankR.Data.Enums;

namespace TankR.Data.Dtos.Identity;

public class RegisterDto
{
    // Identity
    public string Email { get; set; }
    public string Password { get; set; }

    // User profile
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public UserRole Role { get; set; }
}