using EShop.Shared.Enums;
using Order.Domain.Core;


namespace Order.Domain.OrderAggregate;

public class Order : AuditableEntity, IAggregateRoot
{

    private readonly List<OrderItem> _orderItems = new();
    
    public Order() { }

    public Order(int? buyerId, Address address)
    {
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public void UpdateStatus(OrderStatus orderStatus)
    {
        Status = orderStatus;
    }


    
    public Order(int id,int? buyerId, Address address)
    {
        Id = id;
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }
    public int? BuyerId { get; private set; } = default!;

    public Address Address { get; private set; } = null!;

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
  
    public Guid CorrelationId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? CanceledAtUtc { get; private set; }
    public string? CancelReason { get; private set; }
    
    public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);


    public void AddOrderItem(int productId, string productName, decimal price, string? pictureUrl)
    {
        pictureUrl ??= string.Empty;
        if (_orderItems.Any(x => x.ProductId == productId)) return;
        _orderItems.Add(new OrderItem(productId, productName, pictureUrl, price));
    }
    
    public void MarkAwaitingPayment()
    {
        if (Status == OrderStatus.Canceled)
            throw new InvalidOperationException("İptal edilen sipariş Stok Ayrılımaz taşınamaz.");

        if (Status == OrderStatus.AwaitingPayment || Status == OrderStatus.Paid || Status == OrderStatus.Completed)
            return; 

        Status = OrderStatus.AwaitingPayment;
    }

    public void MarkAsPaid(DateTime paidAtUtc)
    {
        if (Status == OrderStatus.Canceled)
            throw new InvalidOperationException("İptal edilen sipariş ödenmiş olarak işaretlenemez.");

        if (Status == OrderStatus.Paid || Status == OrderStatus.Completed)
            return; 

      
        if (Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException($"{Status} durumundan Ödendi durumuna Geçersiz.");

        Status = OrderStatus.Paid;
        PaidAtUtc = paidAtUtc;
    }

    public void CompleteIfNeeded(DateTime completedAtUtc)
    {
        if (Status == OrderStatus.Completed) return;
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException($"Yalnızca ödenmiş siparişler tamamlanabilir. Güncel: {Status}");

        Status = OrderStatus.Completed;
    }

    public void Cancel(string reason, DateTime canceledAtUtc)
    {
        if (Status == OrderStatus.Canceled) return; 
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Tamamlanan sipariş iptal edilemez.");

        Status = OrderStatus.Canceled;
        CancelReason = reason;
        CanceledAtUtc = canceledAtUtc;
    }


}
