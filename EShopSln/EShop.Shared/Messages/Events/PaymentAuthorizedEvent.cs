using EShop.Shared.Dtos;

namespace EShop.Shared.Messages.Events;

public sealed class PaymentAuthorizedEvent
{
 
 public  Guid CorrelationId { get; init; }
  public  string BuyerId { get; init; }
   public string Province { get; init; }
  public  string District { get; init; }
   public string Street { get; init; }
   public string Line { get; init; }
   public string ZipCode { get; init; }
  public  IReadOnlyList<OrderItemDto> OrderItems { get; init; }
    public string PaymentId { get; init; }
}