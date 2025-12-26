using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IStationPhotoRepo
{
    public Task<IEnumerable<StationPhoto>?> GetAll();
    public Task<IEnumerable<StationPhoto>?> GetAllByStationId(int stationId);
    public Task<StationPhoto?> GetPhotoById(int photoId);
    public Task SavePhoto(StationPhoto photo);
    public Task DeletePhoto(StationPhoto photo);
}