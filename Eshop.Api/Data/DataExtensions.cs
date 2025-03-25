using Microsoft.EntityFrameworkCore;

namespace Eshop.Api.Data;

public static class DataExtensions
{
    //code z videa od https://www.youtube.com/@juliocasal
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EshopContext>();
        dbContext.Database.Migrate();
    }
}
// nemusim psát manuálně "dotnet ef database update" => aplikují se všechny čekající migrace