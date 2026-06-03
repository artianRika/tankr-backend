namespace TankR.Data.Dtos;

public class LoyaltySummaryDto
{
    public int Balance { get; set; }
    public int PointsPerDiscountBlock { get; set; }
    public decimal DiscountMkdPerBlock { get; set; }
    public IEnumerable<LoyaltyHistoryEntryDto> History { get; set; } = [];
}
