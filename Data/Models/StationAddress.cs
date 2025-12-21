using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TankR.Data.Enums;

namespace TankR.Data.Models;

[Index(nameof(StationId), IsUnique = true)]
public class StationAddress
{
    [Key]
    public int Id { get; set; }

    public int StationId { get; set; }
    
    public string? Street { get; set; }
    public string? StreetNumber { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string PostalCode { get; set; }
    
    [Required]
    public CountryCode Country { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,7)")]
    public decimal Lat { get; set; }
    [Required]
    [Column(TypeName = "decimal(10,7)")]
    public decimal Lng { get; set; }
}