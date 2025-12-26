using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IStationFuelPriceRepo
{
    
    Task<StationFuelPrice?> Get(int stationId, int fuelTypeId);
    Task<IEnumerable<StationFuelPrice>?> GetAllByStation(int stationId);
    Task<IEnumerable<StationFuelPrice>?> GetAll();
    

    Task SetPrice(int stationId, int fuelTypeId, decimal price);    
    Task UpdatePrice(int stationId, int fuelTypeId, decimal newPrice);
}