using Eshop.Api.Entities;

namespace Eshop.Api.Data;
public static class DataInitializer
{
    public static void SeedAdmin(EshopContext context) //DI EshopContext z kontejneru
    {
        // Kontrola, zda existuje hardcoded admin
        if (!context.Users.Any(u => u.IsHardcodedAdmin))
        {
            var hardcodedAdmin = new User
            {
                Name = "Super Admin",
                Email = "admin@gmail.com",
                Password = "securepassword",
                Role = "Admin",
                Phone = "010101010",
                Town = "Praha",
                Street = "Neznámá ulice",
                Psc = "11000",
                IsHardcodedAdmin = true
            };

            context.Users.Add(hardcodedAdmin);
            context.SaveChanges();
        }
    }

}
