using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IStationEmployeeRepo
{
    bool Exists(int stationId, int userId);

    void Assign(int stationId, int userId);
    void Remove(int stationId, int userId);

    IEnumerable<User> GetEmployeesByStation(int stationId);

}