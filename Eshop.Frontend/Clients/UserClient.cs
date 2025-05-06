using System.Net.Http.Json;
using System.Text.Json;
using Eshop.Frontend.Models;
using Eshop.Frontend.Utils;
using Microsoft.JSInterop;

public class UserClient(HttpClient httpClient, IJSRuntime jsRuntime, AuthService authService)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly IJSRuntime jsRuntime = jsRuntime;
    private readonly AuthService authService = authService;

    public async Task AddUserAsync(User user)
    {
        // Posílání tokenu do API
        await SendToken();

        Console.WriteLine($"Registrace uživatele: {JsonSerializer.Serialize(user)}");
        var response = await httpClient.PostAsJsonAsync("/api/user", user);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadFromJsonAsync<EmailResponse>();

            if (errorContent != null)
            {
                var AllowedDomains = string.Join(", ", errorContent.AllowedDomains);
                var AllowedWebs = string.Join(", ", errorContent.AllowedWebs);
                throw new Exception($"{errorContent.Message}-------------------Povolené weby: {AllowedWebs} -------------------Povolené domény: {AllowedDomains}");
            }
        }
    }

    public async Task<User> GetCurrentUserAsync()
    {
        //Posílaní Tokenu do API
        await SendToken();

        var user = await httpClient.GetFromJsonAsync<User>($"api/user/current-user");
        if (user == null)
        {
            throw new Exception("Uživatel nebyl nalezen.");
        }
        return user;
    }

    //Admin
    public async Task<List<User>> GetAllUsersAsync()
    {
        //Posílaní Tokenu do API
        await SendToken();

        var users = await httpClient.GetFromJsonAsync<List<User>>("/api/user");
        if (users == null)
        {
            throw new Exception("Seznam uživatelů je prázdný");
        }
        return users;
    }

    //Admin
    public async Task UpdateUserRoleAsync(ChangeRoleRequest request)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.PutAsJsonAsync("/api/user/change-role", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception(errorMessage);
        }
    }

    public async Task UpdateUserDataAsync(UpdatedUserRequest updatedUser)
    {
        //Získání ID uživatele z tokenu
        var UserId = await authService.GetUserIdAsync();
        
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.PutAsJsonAsync($"/api/user/update-user-data", updatedUser);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadFromJsonAsync<EmailResponse>();

            if (errorContent != null)
            {
                var AllowedDomains = string.Join(", ", errorContent.AllowedDomains);
                var AllowedWebs = string.Join(", ", errorContent.AllowedWebs);
                throw new Exception($"{errorContent.Message}-------------------Povolené weby: {AllowedWebs} -------------------Povolené domény: {AllowedDomains}");
            }
        }

        //Musis znovu nacist token aby měl aktualní hodnoty
        var newToken = await response.Content.ReadAsStringAsync();

        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "authToken", newToken);

    }

    public async Task DeleteUserById(int UserId)
    {
        //Posílaní Tokenu do API
        await SendToken();

        var response = await httpClient.DeleteAsync($"/api/user/delete-by-id/{UserId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Chyba při mazání uživatele: {error}");
        }
    }

    private async Task SendToken()
    {
        //Posílání tokenu do API kvůli authentifikaci
        var token = await authService.GetAuthTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
    public class EmailResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string> AllowedDomains { get; set; } = new List<string>();
        public List<string> AllowedWebs { get; set; } = new List<string>();
    }

}
