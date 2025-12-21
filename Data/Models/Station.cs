using System.ComponentModel.DataAnnotations;

namespace TankR.Data.Models;

public class Station
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }

    public StationAddress Address { get; set; }
    
    public string? LogoUrl { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
