using TankR.Data.Dtos.Address;

namespace TankR.Data.Dtos.Stations;

public class StationDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public AddressDto Address { get; set; }
}
