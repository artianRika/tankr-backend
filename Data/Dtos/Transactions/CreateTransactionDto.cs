namespace TankR.Data.Dtos.Transactions;

public class CreateTransactionDto
{
    public int StationId { get; set; }

    public int CustomerId { get; set; }
    public int FuelTypeId { get; set; }
    
    public decimal Liters { get; set; }
  
    public DateTime CreatedAt { get; set; }
}