using System.Net.Http.Json; //PutAS, PostAs...
using System.Text.Json; //Serializer
using Eshop.Frontend.Models;
using Eshop.Frontend.Utils;


namespace Eshop.Frontend.Clients;

public class ProductClient(HttpClient httpClient, AuthService authService)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly AuthService authService = authService;

    public async Task<List<Product>> GetAllProductsAsync()
    {
        //Posílaní Tokenu do API
        await SendToken();

        var products = await httpClient.GetFromJsonAsync<List<Product>>("/api/product"); //lze použít i .GetAsync(url) a to pak nabídne možnosti jako: .StatusCode, .IsSuccesStatusCode atd., já zvolil variantu kdy vracím jen tělo odpovědi
        if (products == null)
        {
            throw new Exception("Produkty nebyly nalezeny");
        }
        return products;
    }

    public async Task<List<Product>> GetFilteredProductsAsync(string searchString)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var filteredProducts = await httpClient.GetFromJsonAsync<List<Product>>($"/api/product/filtered-by/{searchString}");
        if (filteredProducts == null)
        {
            throw new Exception("Produkty pro tento search string nebyly nalezeny.");
        }
        return filteredProducts;
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var product = await httpClient.GetFromJsonAsync<Product>($"http://localhost:5042/api/product/{productId}");
        if (product == null)
        {
            throw new Exception("Produkt nebyl nalezen.");
        }
        return product;
    }

    //Admin
    public async Task AddProductAsync(Product product)
    {
        //Posílaní Tokenu do API
        await SendToken();

        Console.WriteLine($"Sending: {JsonSerializer.Serialize(product)}");
        var response = await httpClient.PostAsJsonAsync("/api/product", product);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při přidání produktu: {errorMessage}");
        }
    }

    //Admin
    public async Task DeleteProductByIdAsync(int productId)
    {
        //Posílaní Tokenu do API
        await SendToken();

        await httpClient.DeleteAsync($"http://localhost:5042/api/product/{productId}");
    }

    //Admin
    public async Task UpdateProductAsync(int productId, Product updatedProduct)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.PutAsJsonAsync($"/api/product/{productId}", updatedProduct);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při aktualizaci produktu: {errorMessage}");
        }
    }

    private async Task SendToken()
    {
        //Posílání tokenu do API kvůli authentifikaci
        var token = await authService.GetAuthTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            //nastavuje bearer token do hlavičky požadavků odesílaných směr API
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<string>> GetAvailableImagePathsAsync()
    {
        await SendToken();
        
        var paths = await httpClient.GetFromJsonAsync<List<string>>("api/image/list");
        return paths ?? new();
    }



}
