using TankR.Data.Enums;

namespace TankR.Data.Dtos.UserAddresses;

public class UserAddressDto
{
    public int Id { get; set; }

    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string City { get; set; }
    
    public decimal Lat  { get; set; }
    public decimal Lng { get; set; }

    public string PostalCode { get; set; }
    public CountryCode Country { get; set; }
}
