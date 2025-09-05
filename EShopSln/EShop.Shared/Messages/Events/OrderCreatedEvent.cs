namespace EShop.Shared.Messages.Events;

public class OrderCreatedEvent
{ 
    public Guid CorrelationId { get; init; }
    public int OrderId { get; init; }
    public int BuyerId { get; init; }
    public decimal Total { get; init; }
}