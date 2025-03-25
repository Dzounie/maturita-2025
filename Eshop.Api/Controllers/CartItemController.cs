using Eshop.Api.Entities;
using Eshop.Api.Data;
using Microsoft.AspNetCore.Mvc; // základ pro webové controllery - používání atributů (Authorize...), Controllbase inheritance, návratové typy
using Microsoft.EntityFrameworkCore; //umoznuje práci s databází (Object-Relational Mapper) přes C# objekty (ToListAsync...)
using Microsoft.AspNetCore.Authorization; //token using

namespace Eshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartItemController(EshopContext context) : ControllerBase
{
    private readonly EshopContext context = context; //Dependency Injection v konstruktoru, díky které komunikujeme s databází (přes EshopContext)

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCartItemAsync(CartItem cartItem)
    {
        await context.Carts.AddAsync(cartItem);
        await context.SaveChangesAsync();
        Console.WriteLine($"Přijatá položka objednávky: {cartItem.ProductId}");

        return Ok("Položka byla přidána.");
    }

    [Authorize]
    [HttpGet]
    public async Task<IEnumerable<CartItem>> GetCartItemsAsync() //dalo by se zabalit do <ActionResult<IEnumerable<CartItem>>>, ale mně to takhle stačí
    {
        return await context.Carts.ToListAsync();
    }

    [Authorize]
    [HttpGet("byUser/{userId}")]
    public async Task<IEnumerable<CartItem>> GetCartItemsByUserIdAsync(int userId)
    {
        var cartItems = await context.Carts
            .Where(x =>x.UserId == userId)
            .ToListAsync();
        return cartItems;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCartItemByIdAsync(int id)
    {
        var item = await context.Carts.FindAsync(id);

        if (item == null)
        {
            return NotFound("Položka nebyla nalezena.");
        }

        return Ok(item);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCartItemAsync(int id)
    {
        var item = await context.Carts.FindAsync(id);

        if (item == null)
        {
            return NotFound("Položka nebyla nalezena.");
        }

        context.Carts.Remove(item);
        await context.SaveChangesAsync();

        return Ok("Položka byla úspěšně odstraněna.");
    }


    [Authorize]
    [HttpPut("update/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItemAsync(int cartItemId, [FromBody] CartItemUpdateRequest request)
    {
        var cartItem = await context.Carts.FindAsync(cartItemId);
        if (cartItem == null)
        {
            return NotFound("Položka v košíku nebyla nalezena.");
        }

        cartItem.Size = request.Size;
        cartItem.Quantity = request.Quantity;
        await context.SaveChangesAsync();

        return Ok("Položka byla úspěšně upravena.");
    }

    public class CartItemUpdateRequest
    {
        public required string Size { get; set; }
        public required int Quantity { get; set; }
    }
}