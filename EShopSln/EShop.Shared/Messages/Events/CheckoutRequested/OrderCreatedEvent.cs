namespace EShop.Shared.Messages.Events.CheckoutRequested;

public interface OrderCreatedEvent
{
   public Guid CorrelationId { get; init; }
   public int OrderId { get; init; }
   public int BuyerId { get; init; }
    public decimal Total { get; init; }
}