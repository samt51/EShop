using EShop.Shared.Dtos;

namespace EShop.Shared.Messages.Events;

public sealed class PaymentAuthorizedEvent
{
 public string BuyerId { get; init; } = string.Empty;

 public string Province { get; init; } = string.Empty;
 public string District { get; init; } = string.Empty;
 public string Street  { get; init; } = string.Empty;
 public string Line    { get; init; } = string.Empty;
 public string ZipCode { get; init; } = string.Empty;

 public List<PaymentAuthorizedOrderItem> OrderItems { get; init; } = new();

 public string PaymentId { get; init; } = string.Empty;

 // Serileştiriciler için parametresiz ctor
 public PaymentAuthorizedEvent() { }
}
public sealed class PaymentAuthorizedOrderItem
{
 public int    ProductId   { get; init; }
 public string ProductName { get; init; } = string.Empty;
 public decimal Price      { get; init; }
 public string PictureUrl  { get; init; } = string.Empty;
}