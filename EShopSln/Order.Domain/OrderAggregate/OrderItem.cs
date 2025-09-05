using Order.Domain.Core;

namespace Order.Domain.OrderAggregate;

public class OrderItem : AuditableEntity
{
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }=string.Empty;
    public string PictureUrl { get; private set; }= string.Empty;
    public Decimal Price { get; private set; }
    
    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public OrderItem()
    {
    }
    public OrderItem(int productId, string productName, string pictureUrl, decimal price)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        Price = price;
    }
    
    public OrderItem(int productId, string productName, string pictureUrl, decimal price,int orderId)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        Price = price;
        OrderId = orderId;
    }

    public OrderItem(int id,int productId, string productName, string pictureUrl, decimal price,int orderId)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        Price = price;
        OrderId = orderId;
        Id = id;
    }

    public void UpdateOrderItem(string productName, string pictureUrl, decimal price)
    {
        ProductName = productName;
        Price = price;
        PictureUrl = pictureUrl;
    }
}