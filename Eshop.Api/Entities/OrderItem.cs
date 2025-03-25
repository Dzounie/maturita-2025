namespace Eshop.Api.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required int OrderId { get; set; }
    public required int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string Size { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public required decimal TotalPrice { get; set; }
}