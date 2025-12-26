using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TankR.Data.Models;


public class Transaction
{
    [Key] 
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }
    public Station Station { get; set; }


    [Required]
    public int CashierId { get; set; }
    public User Cashier { get; set; }

    [Required]
    public int CustomerId { get; set; }
    public User Customer { get; set; }

    [Required]
    public int FuelTypeId { get; set; }
    public FuelType FuelType { get; set; }

    [Required]
    public decimal Liters { get; set; }

    [Required]
    public decimal PricePerLiter { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public int PointsEarned { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}