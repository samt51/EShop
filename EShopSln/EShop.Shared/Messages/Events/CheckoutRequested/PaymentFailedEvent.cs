namespace EShop.Shared.Messages.Events.CheckoutRequested;

public class PaymentFailedEvent
{
  public  Guid CorrelationId { get; }
   public string BuyerId { get; init; }
   public string Reason { get; init; }
}