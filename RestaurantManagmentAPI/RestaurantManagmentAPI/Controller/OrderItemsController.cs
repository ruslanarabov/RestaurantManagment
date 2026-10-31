
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagmentAPI.Data;
using RestaurantManagmentAPI.Models;

namespace RestaurantManagmentAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrderItemsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/orderitems
    [HttpPost]
    public async Task<IActionResult> Add(OrderItem orderItem)
    {
        if (orderItem.OrderId <= 0)
            return BadRequest("Order ID must be greater than zero.");

        if (orderItem.ProductId <= 0)
            return BadRequest("Product ID must be greater than zero.");

        if (orderItem.Quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        var order = await _context.Orders
            .FindAsync(orderItem.OrderId);

        if (order == null)
            return NotFound("Order not found.");

        var product = await _context.Products
            .FindAsync(orderItem.ProductId);

        if (product == null)
            return NotFound("Product not found.");

        if (!product.IsAvailable)
            return BadRequest("Product is not available.");

        var existingItem = await _context.OrderItems
            .FindAsync(orderItem.OrderId, orderItem.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += orderItem.Quantity;
        }
        else
        {
            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Product added to order successfully.",
            OrderId = orderItem.OrderId,
            ProductId = orderItem.ProductId,
            Quantity = existingItem?.Quantity ?? orderItem.Quantity
        });
    }

    // DELETE: api/orderitems
    [HttpDelete("{orderId}/{productId}")]
    public async Task<IActionResult> Remove(int orderId, int productId)
    {
        if (orderId <= 0)
            return BadRequest("Order ID must be greater than zero.");

        if (productId <= 0)
            return BadRequest("Product ID must be greater than zero.");

        var orderItem = await _context.OrderItems
            .FindAsync(orderId, productId);

        if (orderItem == null)
            return NotFound("Order item not found.");

        _context.OrderItems.Remove(orderItem);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
