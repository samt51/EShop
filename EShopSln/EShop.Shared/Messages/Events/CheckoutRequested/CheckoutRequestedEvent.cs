using EShop.Shared.Dtos;

namespace EShop.Shared.Messages.Events.CheckoutRequested;

public sealed class CheckoutRequestedEvent
{
  public string BuyerId { get; init; } = string.Empty;
  public int BasketId { get; set; }
  public List<CheckoutRequestedItem> Items { get; init; } = new();

  public CheckoutRequestedEvent() { }
}

public sealed class CheckoutRequestedItem
{
  public int    ProductId   { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public decimal Price      { get; init; }
  public string PictureUrl  { get; init; } = string.Empty;
  public int BasketId { get; set; }
  public int Quantity { get; set; }
}