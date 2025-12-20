namespace TankR.Data.Dtos.Transactions;

public class TransactionDto
{
    public int Id { get; set; }

    public int StationId { get; set; }
    public int FuelTypeId { get; set; }

    public decimal Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalPrice { get; set; }

    public int PointsEarned { get; set; }
    public DateTime CreatedAt { get; set; }
}
