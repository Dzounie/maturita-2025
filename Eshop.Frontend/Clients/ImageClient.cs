// Clients/ImageClient.cs
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;

namespace Eshop.Frontend.Clients;

public class ImageClient(HttpClient httpClient)
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<string> UploadImageAsync(IBrowserFile file)
    {
        var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10_000_000));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.Name);

        var response = await httpClient.PostAsync("/api/image/upload", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při nahrávání obrázku: {error}");
        }

        var relativePath = await response.Content.ReadAsStringAsync();

        // ZDE upravujeme návratovou hodnotu:
        var absolutePath = $"http://localhost:5042/{relativePath}";

        return absolutePath;
    }

}
