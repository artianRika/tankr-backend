using TankR.Data.Enums;

namespace TankR.Data.Dtos.Address;

public class AddressDto
{
    public int Id { get; set; }

    public string Street { get; set; }
    public string City { get; set; }

    public string PostalCode { get; set; }
    public CountryCode Country { get; set; }
}
