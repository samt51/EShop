namespace EShop.Shared.Messages.Events;

public sealed record OrderPaidEvent(
    Guid   CorrelationId,
    int    OrderId,
    int    BuyerId,
    decimal Total,
    DateTime PaidAtUtc
);