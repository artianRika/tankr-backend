using TankR.Data.Dtos.StationEmployees;
using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IStationEmployeeRepo
{
    Task<Boolean> Exists(int stationId, int userId);

    Task Assign(StationEmployee stationEmployee);
    Task Remove(int stationId, int userId);

    Task<IEnumerable<StationEmployee>?> GetEmployeesByStation(int stationId);

}