using TankR.Data.Dtos.Address;
using TankR.Data.Enums;

namespace TankR.Data.Dtos;

public class UserDetailsDto
{
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Email { get; set; }
    public UserRole Role { get; set; }

    public AddressDto Address { get; set; }
}
