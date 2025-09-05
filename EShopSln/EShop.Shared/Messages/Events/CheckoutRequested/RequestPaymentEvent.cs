using EShop.Shared.Dtos;

namespace EShop.Shared.Messages.Events.CheckoutRequested;

public interface RequestPaymentEvent
{
    Guid CorrelationId { get; }
    string BuyerId { get; }
    string PaymentId { get; }
    IReadOnlyList<OrderItemDto> OrderItems { get; }
}