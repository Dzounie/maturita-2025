namespace Eshop.Api.Entities;

public class Order
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required decimal TotalPrice { get; set; }


    public required OrderStatus Status { get; set; }
    public User User { get; set; } = null!;
    public int StatusId { get; set; }

}