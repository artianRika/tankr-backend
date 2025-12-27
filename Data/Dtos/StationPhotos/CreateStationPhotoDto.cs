using System.ComponentModel.DataAnnotations;
using TankR.Data.Models;

namespace TankR.Data.Dtos.StationPhotos;

public class CreateStationPhotoDto
{
    public int StationId { get; set; }
    public IFormFile? Image { get; set; }

}