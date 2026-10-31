
using Microsoft.AspNetCore.Mvc;
using RestaurantManagmentAPI.Data;
using Microsoft.EntityFrameworkCore;
using RestaurantManagmentAPI.Models;

namespace RestaurantManagmentAPI.Controller;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/category
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories);
    }

    // GET: api/category/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("ID must be greater than zero.");

        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound("Category not found.");

        return Ok(category);
    }

    // POST: api/category
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            return BadRequest("Category name is required.");

        if (category.Name.Length > 50)
            return BadRequest("Category name cannot exceed 50 characters.");

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = category.Id }, category);
    }

    // PUT: api/category/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category updatedCategory)
    {
        if (id <= 0)
            return BadRequest("ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(updatedCategory.Name))
            return BadRequest("Category name is required.");

        if (updatedCategory.Name.Length > 50)
            return BadRequest("Category name cannot exceed 50 characters.");

        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound("Category not found.");

        category.Name = updatedCategory.Name;

        await _context.SaveChangesAsync();

        return Ok(category);
    }
}
