using System.IdentityModel.Tokens.Jwt;
using Microsoft.JSInterop;

namespace Eshop.Frontend.Utils;

public class AuthService(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime = jsRuntime;

    public async Task<bool> IsUserAdmin()
    {
        var token = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");

        if (string.IsNullOrEmpty(token)) return false;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var role = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;

        return role == "Admin";
    }

    public async Task<string> GetAuthTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
    }
}