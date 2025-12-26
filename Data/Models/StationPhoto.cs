using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TankR.Data.Models;


[Index(nameof(ImagePath), IsUnique = true)]
public class StationPhoto
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }

    public Station Station { get; set; }

    [Required]
    public string ImagePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}