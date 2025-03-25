using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Eshop.Frontend;
using Eshop.Frontend.Components;
using Eshop.Frontend.Utils;
using Eshop.Frontend.Models;
using Eshop.Frontend.Clients;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//--------------------------------------------------------------------------------NEPOUZIVAM-------------------------------------------------------------------------------------\\
// Registrace HttpClientFactory služby na komunikaci s API, pak jsem změnil způsob komunikace. 
builder.Services.AddHttpClient("EshopAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5042"); //port backendu...pak by si mohl volat jenom httpClient....("/api/user") nemusel by si psat porad http://localhost
});
//Pokud by šlo o rozsáhlejší aplikaci s větší zátěží, použil bych HttpClientFactory kvůli lepší správě připojení a možnosti pracovat s více API.
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\\


// Registrace HttpClient služby pro Blazor aplikaci na komunikai s API. POUZIVAM
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("http://localhost:5042") }); //Url API, teda v Clients ted nemusis vypisovat porad htttp://localhost:...
//místo "service provider (sp)" pouzivam "_" , jelikož service provider bych nijak nevyužil stačí mi jen vytvořit instanci HttpClientu a nastavit mu BaseAddress


// Přidání Services
builder.Services.AddScoped<NavigationService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<UserClient>();
builder.Services.AddScoped<CartItemClient>();
builder.Services.AddScoped<ProductClient>();
builder.Services.AddScoped<OrderClient>();
builder.Services.AddScoped<ImageClient>();
builder.Services.AddScoped<TokenManager>();


await builder.Build().RunAsync();