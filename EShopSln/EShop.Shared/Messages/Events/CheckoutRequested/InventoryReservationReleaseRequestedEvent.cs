namespace EShop.Shared.Messages.Events.CheckoutRequested;

public interface InventoryReservationReleaseRequestedEvent
{
  public  string BuyerId { get; init; }
 public   Guid BasketId { get; init; }
}