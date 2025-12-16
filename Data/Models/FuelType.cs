using System.ComponentModel.DataAnnotations;

namespace TankR.Data.Models;

public class FuelType
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }    
    
}