namespace TankR.Services.Interfaces;

public interface IFreeImageService
{
    Task<string> UploadAsync (IFormFile file);
}