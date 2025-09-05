using Order.Domain.Core;

namespace Order.Domain.OrderAggregate;

public class Order : AuditableEntity, IAggregateRoot
{

    private readonly List<OrderItem> _orderItems = new();

    // EF Core için parametresiz ctor (materialization)
    public Order() { }

    public Order(int? buyerId, Address address)
    {
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    
    public Order(int id,int? buyerId, Address address)
    {
        Id = id;
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }
    public int? BuyerId { get; private set; } = default!;

    public Address Address { get; private set; } = null!; // ctor’da atanıyor

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public void AddOrderItem(int productId, string productName, decimal price, string? pictureUrl)
    {
        pictureUrl ??= string.Empty;
        if (_orderItems.Any(x => x.ProductId == productId)) return;
        _orderItems.Add(new OrderItem(productId, productName, pictureUrl, price));
    }
    public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);

}
