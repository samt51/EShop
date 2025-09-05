namespace EShop.Shared.Messages.Events.CheckoutRequested;


public interface InventoryReservationFailedEvent
{
  public  string BuyerId { get; init; }
  public  Guid BasketId { get; init; }
   public string Reason { get; init; }
}