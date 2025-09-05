namespace EShop.Shared.Messages.Events.CheckoutRequested;

public sealed class PaymentFailedEvent
{
    public string BuyerId { get; init; } = string.Empty;
    public string Reason  { get; init; } = string.Empty;

    public PaymentFailedEvent() { }
}