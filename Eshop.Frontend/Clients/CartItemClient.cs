using Eshop.Frontend.Models;
using System.Text.Json; //JsonSerializer
using System.Net.Http.Json; //PostAsJsonAsync...
using Eshop.Frontend.Utils;

namespace Eshop.Frontend.Clients;

public class CartItemClient(HttpClient httpClient, AuthService authService)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly AuthService authService = authService;

    public async Task AddCartItemAsync(CartItem cartItem)
    {
        //Posílaní Tokenu do API
        await SendToken();

        Console.WriteLine($"Vybranná položka: {JsonSerializer.Serialize(cartItem)}");
        var response = await httpClient.PostAsJsonAsync("/api/cartItem", cartItem);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při přidání produktu: {errorMessage}");
        }
    }


    public async Task<List<CartItem>> GetCartItemByUserIdAsync() //(GetCartItemsByUserIdAsync)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var cartItems = await httpClient.GetFromJsonAsync<List<CartItem>>($"/api/cartItem");
        if (cartItems == null)
        {
            throw new Exception("Chyba při načítání produktů");
        }

        return cartItems;
    }

    public async Task<CartItem> GetCartItemByIdAsync(int cartItemId)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var cartItem = await httpClient.GetFromJsonAsync<CartItem>($"/api/cartItem/{cartItemId}"); //lze použít i .GetAsync(url) a to pak nabídne možnosti jako: .StatusCode, .IsSuccesStatusCode atd., já zvolil variantu kdy vracím jen tělo odpovědi
        if (cartItem == null)//pak musíme použít cartItem == null a nemůžeme kontrolovat StatusCode
        {
            throw new Exception("Chyba při načítání produktu");
        }

        return cartItem;
    }

    public async Task DeleteCartItemAsync(int cartItemId)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.DeleteAsync($"/api/cartItem/{cartItemId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při mazání produktu: {errorMessage}");
        }
    }

    public async Task UpdateCartItemAsync(int cartItemId, CartItemUpdateRequest updatedCartItem)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.PutAsJsonAsync($"/api/cartItem/update/{cartItemId}", updatedCartItem);

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

}