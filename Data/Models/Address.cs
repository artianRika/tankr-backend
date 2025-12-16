using System.ComponentModel.DataAnnotations;
using TankR.Data.Enums;

namespace TankR.Data.Models;

public class Address
{
    [Key]
    public int Id { get; set; }

    public string? Street { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string PostalCode { get; set; }
    
    public CountryCode Country { get; set; }

    [Required]
    public decimal Lat { get; set; }
    [Required]
    public decimal Lng { get; set; }
}
