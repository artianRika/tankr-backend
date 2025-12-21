using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IStationAddressRepo
{
    Task<StationAddress?> GetById(int id);
    Task<IEnumerable<StationAddress>?> GetAll();
    
    Task Update(StationAddress address);
    Task Add(StationAddress address);
}