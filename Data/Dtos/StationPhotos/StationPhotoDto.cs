using Microsoft.EntityFrameworkCore;
using TankR.Data.Dtos.Stations;

namespace TankR.Data.Dtos.StationPhotos;


public class StationPhotoDto
{
    public int Id { get; set; }

    public int StationId { get; set; }

    public StationDto Station { get; set; }

    public string ImagePath { get; set; }

}