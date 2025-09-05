using EShop.Shared.Dtos;

namespace EShop.Shared.Messages.Events.CheckoutRequested;

public class CheckoutRequestedEvent
{
  public  string BuyerId { get; init; }
  public Guid BasketId { get; init; }
  public  IReadOnlyList<CheckoutItem> Items { get; init; }
}