using System.ComponentModel.DataAnnotations;

namespace Eshop.Frontend.Models;

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    [Required]
    public required string Size { get; set; }
    [Required]
    [Range(1, 10)]
    public required int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}