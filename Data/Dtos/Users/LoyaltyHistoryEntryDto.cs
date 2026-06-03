namespace TankR.Data.Dtos;

public class LoyaltyHistoryEntryDto
{
    public string Type { get; set; } = "";
    public int Points { get; set; }
    public decimal? DiscountMkd { get; set; }
    public int TransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
}
