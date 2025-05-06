using System;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Frontend.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    [Length(4, 50, ErrorMessage = "Jméno musí být 4-50 charakterů dlouhé.")]
    public string Name { get; set; } = string.Empty;
    [Required]
    [Range(1, 5000, ErrorMessage = "Cena musí být mezi 1 a 5000 Kč.")]
    public decimal Price { get; set; }
    [Required]
    public string ImageUri { get; set; } = string.Empty;
}
