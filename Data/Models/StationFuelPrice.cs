using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TankR.Data.Models;


[Index(nameof(StationId), nameof(FuelTypeId), IsUnique = true)]
public class StationFuelPrice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }
    public Station Station { get; set; }

    [Required]
    public int FuelTypeId { get; set; }
    public FuelType FuelType { get; set; }

    [Required]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
