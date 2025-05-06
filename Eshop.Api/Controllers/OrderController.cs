using Eshop.Api.Entities;
using Eshop.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; //token using

namespace Eshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(EshopContext context) : ControllerBase
{
    private readonly EshopContext context = context;

    [Authorize]
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrderAsync()
    {
        int userId = int.Parse(User.FindFirst("UserId")!.Value);

        //Nají položky v košíku
        var cartItems = await context.Carts
            .Where(item => item.UserId == userId)
            .ToListAsync();
        if (cartItems == null || cartItems.Count == 0)
        {
            return NotFound("Košík je prázdný");
        }

        var defaultStatus = await context.OrderStatuses.FindAsync(1);
        if (defaultStatus == null) return BadRequest("Výchozí stav objednávky nebyl nalezen.");

        //Vytvořit objednávku a získat orderId  
        var newOrder = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = defaultStatus,
            TotalPrice = cartItems.Sum(x => x.Quantity * x.UnitPrice) + 100 // Celková cena objednávky
        };

        await context.Orders.AddAsync(newOrder);
        await context.SaveChangesAsync();

        //Přepsat cartItems do orderItems a přidat jim orderId
        foreach (var item in cartItems)
        {
            var newOrderItem = new OrderItem
            {
                OrderId = newOrder.Id,
                UserId = item.UserId,
                ProductName = item.ProductName,
                ProductId = item.ProductId,
                Size = item.Size,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice
            };

            await context.OrderItems.AddAsync(newOrderItem);
        }

        context.Carts.RemoveRange(cartItems);
        await context.SaveChangesAsync();

        return Ok(newOrder);
    }

    [Authorize]
    [HttpGet("get-latest-order")]
    public async Task<IActionResult> GetLatestOrdersAsync()
    {
        int userId = int.Parse(User.FindFirst("UserId")!.Value);

        var order = await context.Orders
            .Where(order => order.UserId == userId)
            .Include(order => order.Status) //eager loading
            .OrderByDescending(o => o.CreatedAt) //radi podle data vytvoreni
            .Select(order => new //vybíráš co chceš poslat
            {
                order.Id,
                order.CreatedAt,
                order.TotalPrice,
                order.Status.StatusName
            })
            //.ToListAsync(); kdybys chtěl vracet vsechny
            .FirstOrDefaultAsync(); //najde prvni objednavku

        return Ok(order);
    }


    [Authorize]
    [HttpGet("get-orders")]
    public async Task<IActionResult> GetOrdersAsync()
    {
        int userId = int.Parse(User.FindFirst("UserId")!.Value);

        var order = await context.Orders
            .Where(order => order.UserId == userId)
            .Include(order => order.Status) //eager loading
            .OrderByDescending(o => o.CreatedAt) //radi podle data vytvoreni
            .Select(order => new
            {
                order.Id,
                order.CreatedAt,
                order.TotalPrice,
                order.Status.StatusName //bez Include by EF nic nenačetlo a házelo by null
            })
            .ToListAsync();

        return Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-all-orders")]
    public async Task<IActionResult> GetAllOrdersAsync()
    {
        var orders = await context.Orders
            .Include(o => o.Status) //mohl bych nastavit lazy loading v entitách, ale eager loading je pro mě snažší
            .OrderByDescending(o => o.CreatedAt)
            .Select(order => new
            {
                order.Id,
                order.UserId,
                UserName = order.User.Name,
                order.CreatedAt,
                order.TotalPrice,
                order.Status.StatusName //bez Include by EF nic nenačetlo a házelo by null
            })
            .ToListAsync();

        return Ok(orders);
    }


    [Authorize]
    [HttpGet("get-order-items/{orderId}")]
    public async Task<IActionResult> GetOrderItemsAsync(int orderId)
    {
        var userIdFromToken = int.Parse(User.FindFirst("UserId")!.Value);

        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null || (order.UserId != userIdFromToken && !User.IsInRole("Admin")))
        {
            return NotFound("Objednávka nebyla nalezena.");
        }

        var orderItems = await context.OrderItems
            .Where(item => item.OrderId == orderId)
            .ToListAsync();

        return Ok(orderItems);
    }

    [Authorize]
    [HttpGet("statuses")]
    public async Task<IEnumerable<OrderStatus>> GetOrderStatusesAsync()
    {
        return await context.OrderStatuses.ToListAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update-status/{orderId}")]
    public async Task<IActionResult> UpdateOrderStatusAsync(int orderId, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return NotFound("Objednávka nebyla nalezena.");
        }
        order.StatusId = request.StatusId;
        await context.SaveChangesAsync();

        return NoContent();
    }

    public class UpdateOrderStatusRequest
    {
        public int StatusId { get; set; }
    }

}