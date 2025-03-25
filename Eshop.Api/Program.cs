using Eshop.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Načtení konfigurace JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("JWT SecretKey is not configured properly.");//aby aplikace spadla hned a ne az po prvnim token pozadavku
}
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

//Atentizace a Autentifikace --napsal jsem s pomocí copilotu a přidal valstní RoleClaim
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = ClaimTypes.Role,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

var connString = builder.Configuration.GetConnectionString("Eshop");
// zapíše EshopContext třídu do DI kontejneru
// nastaví typ databáze 
// napojí na connstring ať ví s jakým .db souborem pracovat
builder.Services.AddSqlite<EshopContext>(connString);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Registrace CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5214") // URL frontendu
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

// Přidání hardcoded admina
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EshopContext>();

    DataInitializer.SeedAdmin(context);
}

app.MigrateDb();
app.UseRouting();
app.UseCors("AllowFrontend"); // aktivace CORS politiky před mapováním endpointů
app.UseAuthentication(); // Přidání autentizace
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
app.MapFallbackToFile("index.html");

app.Run();