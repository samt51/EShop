namespace EShop.Shared.Messages.Events;

public sealed record PaymentVoidedEvent(
    Guid   CorrelationId,
    int    OrderId,
    int    PaymentId,
    DateTime VoidedAtUtc
);