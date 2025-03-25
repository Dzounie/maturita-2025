using System.ComponentModel.DataAnnotations;

namespace Eshop.Frontend.Models;

public class CartItemUpdateRequest
{
    [Required(ErrorMessage = "Vyberte prosím velikost.")]
    public string Size { get; set; } = string.Empty;
    [Range(1, 10)]
    public int Quantity { get; set; }
}