namespace EShop.Shared.Messages.Events;

public sealed record PaymentRefundedEvent(
    Guid   CorrelationId,
    int    OrderId,
    int    PaymentId,
    decimal Amount,
    DateTime RefundedAtUtc
);