using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagmentAPI.Data;
using RestaurantManagmentAPI.Models;
using RestaurantManagmentAPI.PublicEnums;

namespace RestaurantManagmentAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/orders
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("ID must be greater than zero.");

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound("Order not found.");

        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.CustomerName))
            return BadRequest("Customer name is required.");

        if (order.CustomerName.Length > 100)
            return BadRequest("Customer name cannot exceed 100 characters.");

        order.Status = OrderStatus.Pending;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = order.Id }, order);
    }
}