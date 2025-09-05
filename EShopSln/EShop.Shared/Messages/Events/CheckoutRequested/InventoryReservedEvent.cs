namespace EShop.Shared.Messages.Events.CheckoutRequested;

public interface ReservedItem
{
    int ProductId { get; }
    int Quantity { get; }
}

public interface InventoryReservedEvent
{
   public string BuyerId { get; init; }
   public Guid BasketId { get; init; }
  public  IReadOnlyList<ReservedItem> Items { get; init; }
}