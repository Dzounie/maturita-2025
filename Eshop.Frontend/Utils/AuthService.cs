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

    public async Task<int> GetUserIdAsync()
    {
        var token = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
        if(string.IsNullOrEmpty(token)) 
            throw new Exception("Token is null or empty");
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId")?.Value;
        
        if (userIdClaim == null) 
            throw new Exception("Claim not found in token");

        if(int.TryParse(userIdClaim, out int userId))
        return userId; throw new Exception("Claim is not a valid integer");

    }

}