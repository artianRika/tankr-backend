using TankR.Data.Dtos.Address;
using TankR.Data.Enums;

public class UserDto
{
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public UserRole Role { get; set; }
    public int LoyaltyPoints { get; set; }
    
    public AddressDto Address { get; set; }
}