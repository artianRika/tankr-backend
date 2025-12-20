using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IStationFuelPriceRepo
{
    StationFuelPrice Get(int stationId, int fuelTypeId);
    IEnumerable<StationFuelPrice> GetByStation(int stationId);

    void SetPrice(StationFuelPrice price);

}