
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagmentAPI.Data;
using RestaurantManagmentAPI.Models;

namespace RestaurantManagmentAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest("ID must be greater than zero.");
        }
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound("Product not found.");
        }

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (product.Price <= 0)
        {
            return BadRequest("Price must be greater than zero.");
        }

        if (product.CategoryId <= 0)
            return BadRequest("Category ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(product.Name))
            return BadRequest("Product name is required.");

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == product.CategoryId);

        if (!categoryExists)
            return BadRequest("Category does not exist.");

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = product.Id }, product);
    }

    // PUT: api/products
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product updatedProduct)
    {
        if (id <= 0)
            return BadRequest("ID must be greater than zero.");

        if (updatedProduct.Price <= 0)
            return BadRequest("Price must be greater than zero.");

        if (updatedProduct.CategoryId <= 0)
            return BadRequest("Category ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(updatedProduct.Name))
            return BadRequest("Product name is required.");

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound("Product not found.");

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == updatedProduct.CategoryId);

        if (!categoryExists)
            return BadRequest("Category does not exist.");

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        product.CategoryId = updatedProduct.CategoryId;
        product.IsAvailable = updatedProduct.IsAvailable;

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("ID must be greater than zero.");

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound("Product not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
