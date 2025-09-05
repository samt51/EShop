namespace Payment.Application.Dtos;

public interface CreateOrderCommand
{
    Guid MessageId { get; }
    Guid CorrelationId { get; }
    string BuyerId { get; }
    AddressDto Address { get; }
    IReadOnlyList<OrderItemDto> OrderItems { get; }
}