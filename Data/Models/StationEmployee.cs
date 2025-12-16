using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TankR.Data.Models;

public class StationEmployee
{

    [Key] 
    public int Id { get; set; }
    
    [Required] 
    public int StationId { get; set; }
    public Station Station { get; set; }

    [Required] 
    public int UserId { get; set; }
    public User User { get; set; }

    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required] 
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}