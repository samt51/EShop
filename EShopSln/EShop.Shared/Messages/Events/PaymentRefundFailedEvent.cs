namespace EShop.Shared.Messages.Events;


public sealed record PaymentRefundFailedEvent(
    Guid   CorrelationId,
    int    OrderId,
    int    PaymentId,
    string Reason,
    DateTime FailedAtUtc
);