namespace TankR.Data.Dtos.Transactions;

public class CreateTransactionDto
{
    public int StationId { get; set; }

    public int CustomerId { get; set; }
    public int FuelTypeId { get; set; }
    
    public decimal Liters { get; set; }

    /// <summary>Redeem loyalty points (multiples of 100 = 10 MKD off per block).</summary>
    public int PointsToRedeem { get; set; }
  
    public DateTime CreatedAt { get; set; }
}