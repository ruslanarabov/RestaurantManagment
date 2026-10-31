using RestaurantManagmentAPI.PublicEnums;

namespace RestaurantManagmentAPI.Models;

public sealed class Order : BaseEntity
{
   public string CustomerName { get; set; }
   public DateTime OrderDate { get; set; }
   public OrderStatus Status { get; set; } = OrderStatus.Pending;
   public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

}