using Eshop.Frontend.Components;
using Microsoft.JSInterop;

namespace Eshop.Frontend.Utils;

public class TokenManager
{
    private readonly IJSRuntime JSRuntime;

    public TokenManager(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime;
    }
    public async Task DeleteLocalStorageAsync()
    {
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "userName");
    }
    public async Task DeleteSessionStorageAsync()
    {
        await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", "authToken");
        await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", "userId");
        await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", "userName");
    }
}