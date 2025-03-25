namespace Eshop.Api.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string Role { get; set; } = "User"; //radši nastavuju tady i v OnModelCreating
        public bool IsHardcodedAdmin { get; set; } = false; 
        public required string Phone { get; set; }
        public required string Password { get; set; }
        public required string Town { get; set; }
        public required string Street { get; set; }
        public required string Psc { get; set; }
    }
}