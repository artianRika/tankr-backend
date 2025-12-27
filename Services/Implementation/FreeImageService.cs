using System.Net.Http.Headers;
using System.Text.Json;
using TankR.Services.Interfaces;

namespace TankR.Services;

public class FreeImageService : IFreeImageService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public FreeImageService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file");

        using var form = new MultipartFormDataContent();
        using var ms = new MemoryStream();

        await file.CopyToAsync(ms);

        var fileContent = new ByteArrayContent(ms.ToArray());
        fileContent.Headers.ContentType =
            MediaTypeHeaderValue.Parse(file.ContentType);

        form.Add(fileContent, "source", file.FileName);
        form.Add(new StringContent(_config["FreeImage:ApiKey"]!), "key");
        form.Add(new StringContent("json"), "format");

        var response = await _http.PostAsync(
            "https://freeimage.host/api/1/upload",
            form
        );

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("image")
            .GetProperty("url")
            .GetString()!;
    }
}