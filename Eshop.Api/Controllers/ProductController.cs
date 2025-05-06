using Eshop.Api.Entities;
using Eshop.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; //token using

namespace Eshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(EshopContext context) : ControllerBase
{
    private readonly EshopContext context = context;

    [Authorize]
    [HttpGet]
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await context.Products.ToListAsync();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductByIdAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound("Produkt nebyl nalezen.");
        }
        return Ok(product);
    }

    [Authorize]
    [HttpGet("filtered-by/{searchString}")]
    public async Task<IEnumerable<Product>> GetProductBySearchAsync(string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return await context.Products.ToListAsync();
        }

        return await context.Products
            .Where(p => p.Name.ToLower().Contains(searchString.ToLower())) // Case-insensitive hledání
            .ToListAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(Product product)
    {
        if (product == null) return BadRequest();
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        Console.WriteLine($"Přijatý produkt: {product.Name}");

        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductByIdAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();

        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] Product updatedProduct)
    {
        if (id != updatedProduct.Id)
        {
            return BadRequest("Product ID neshoda.");
        }

        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        product.ImageUri = updatedProduct.ImageUri;

        Console.WriteLine($"Upravený produkt--> Name: {product.Name}, Price: {product.Price} Kč, ImageUri: {product.ImageUri}.");

        context.Products.Update(product);
        await context.SaveChangesAsync();

        return Ok(product);
    }


}


