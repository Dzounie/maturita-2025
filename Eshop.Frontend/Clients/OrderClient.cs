using Eshop.Frontend.Models;
using System.Net.Http.Json; //PostAsJsonAsync...
using Eshop.Frontend.Utils;

namespace Eshop.Frontend.Clients;

public class OrderClient(HttpClient httpClient, AuthService authService)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly AuthService authService = authService;

    public async Task CreateOrderAsync()
    {
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.PostAsync("/api/order/create-order", null);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při vytváření objednávky: {errorMessage}");
        }
    }


    public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var orderItems = await httpClient.GetFromJsonAsync<List<OrderItem>>($"/api/order/get-order-items/{orderId}"); //lze použít i .GetAsync(url) a to pak nabídne možnosti jako: .StatusCode, .IsSuccesStatusCode atd., já zvolil variantu kdy vracím jen tělo odpovědi
        if (orderItems == null)
        {
            throw new Exception("Objednávka nebyla nalezena.");
        }

        return orderItems;
    }

    public async Task<Order> GetLatestOrderByUserIdAsync()
    {
        //Posílaní Tokenu do API
        await SendToken();

        var order = await httpClient.GetFromJsonAsync<Order>($"/api/order/get-latest-order");
        if (order == null)
        {
            throw new Exception("Nebyly nalezeny žádné objednávky");
        }

        return order;
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync()
    {
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.GetAsync("/api/order/get-orders");

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Chyba z API: {errorText}");
            throw new Exception("Nepodařilo se načíst objednávky.");
        }

        var orders = await response.Content.ReadFromJsonAsync<List<Order>>();

        return orders;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        //Posílaní Tokenu do API
        await SendToken();

        var orders = await httpClient.GetFromJsonAsync<List<Order>>("/api/order/get-all-orders");
        if (orders == null)
        {
            throw new Exception("API nevrátilo žádná data.");
        }
        Console.WriteLine($"Počet objednávek načtených z API: {orders.Count}");

        return orders;
    }

    public async Task<List<OrderStatus>> GetAllOrderStatusesAsync()
    {
        var statuses = await httpClient.GetFromJsonAsync<List<OrderStatus>>("/api/order/statuses");
        return statuses!;
    }

    public async Task UpdateOrderStatusAsync(int orderId, int statusId)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var updateRequest = new { StatusId = statusId };
        var response = await httpClient.PutAsJsonAsync($"http://localhost:5042/api/order/update-status/{orderId}", updateRequest);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při aktualizaci stavu objednávky: {errorMessage}");
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
        Console.WriteLine($"Token: {token}");

    }
}