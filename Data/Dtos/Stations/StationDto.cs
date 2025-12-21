using TankR.Data.Dtos.StationAddresses;
using TankR.Data.Dtos.UserAddresses;

namespace TankR.Data.Dtos.Stations;

public class StationDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string? LogoUrl { get; set; }
    public StationAddressDto Address { get; set; }
}

