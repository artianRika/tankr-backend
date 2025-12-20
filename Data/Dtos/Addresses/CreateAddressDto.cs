using TankR.Data.Enums;

namespace TankR.Data.Dtos.Address;

public class CreateAddressDto
{
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public CountryCode Country { get; set; }

    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    
    public int UserId { get; set; }
}
