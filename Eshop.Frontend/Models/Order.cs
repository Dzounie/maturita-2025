using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eshop.Frontend.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int StatusId { get; set; }

}