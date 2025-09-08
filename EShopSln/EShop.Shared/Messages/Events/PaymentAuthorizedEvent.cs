using EShop.Shared.Dtos;

namespace EShop.Shared.Messages.Events;

public sealed record PaymentAuthorizedEvent :  ICorrelated
{

 public int OrderId { get; set; }
 public int BuyerId { get; init; } 
 public string Province { get; init; } = string.Empty;
 public string District { get; init; } = string.Empty;
 public string Street  { get; init; } = string.Empty;
 public string Line    { get; init; } = string.Empty;
 public string ZipCode { get; init; } = string.Empty;

 public List<PaymentAuthorizedOrderItem> OrderItems { get; init; } = new();

 public string PaymentId { get; init; } = string.Empty;
 public DateTime  OccurredOnUtc { get; set; }
 
 public PaymentAuthorizedEvent() { }
 public Guid CorrelationId { get; init; }
}
public sealed class PaymentAuthorizedOrderItem
{
 public int    ProductId   { get; init; }
 public string ProductName { get; init; } = string.Empty;
 public decimal Price      { get; init; }
 public string? PictureUrl  { get; init; } 
}