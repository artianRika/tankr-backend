namespace TankR.Data.Dtos.StationFuelPrices;

public class SetFuelPriceDto
{
    public int StationId { get; set; }
    public int FuelTypeId { get; set; }

    public decimal Price { get; set; }
}