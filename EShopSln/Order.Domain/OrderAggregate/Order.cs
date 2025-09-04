using Order.Domain.Core;

namespace Order.Domain.OrderAggregate;

public class Order : AuditableEntity, IAggregateRoot
{
    public DateTime CreatedDate { get; private set; }

    public Address Address { get; private set; }

    public int BuyerId { get; private set; }

    private readonly List<OrderItem> _orderItems;

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Order()
    {
    }
    public Order(int buyerId, Address address)
    {
        _orderItems = new List<OrderItem>();
        CreatedDate = DateTime.Now;
        BuyerId = buyerId;
        Address = address;
    }
    public Order(int id,int buyerId, Address address)
    {
        Id = id;
        _orderItems = new List<OrderItem>();
        CreatedDate = DateTime.Now;
        BuyerId = buyerId;
        Address = address;
    }
    public Order(int buyerId, Address address,List<OrderItem> orderItems)
    {
        _orderItems = orderItems;
        CreatedDate = DateTime.Now;
        BuyerId = buyerId;
        Address = address;
    }

    public void AddOrderItem(int productId, string productName, decimal price, string pictureUrl)
    {
        var existProduct = _orderItems.Any(x => x.ProductId == productId);

        if (!existProduct)
        {
            var newOrderItem = new OrderItem(productId, productName, pictureUrl, price);
            _orderItems.Add(newOrderItem);
        }
    }

    public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);
}