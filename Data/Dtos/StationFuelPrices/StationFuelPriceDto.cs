namespace TankR.Data.Dtos.StationFuelPrices;

public class StationFuelPriceDto
{
    public int StationId { get; set; }
    public int FuelTypeId { get; set; }

    public decimal Price { get; set; }
}