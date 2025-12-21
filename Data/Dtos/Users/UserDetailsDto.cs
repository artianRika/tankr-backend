using TankR.Data.Dtos.UserAddresses;
using TankR.Data.Enums;

namespace TankR.Data.Dtos;

public class UserDetailsDto
{
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Email { get; set; }
    public UserRole Role { get; set; }

    public UserAddressDto Address { get; set; }
}
