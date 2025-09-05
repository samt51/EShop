namespace EShop.Shared.Messages.Commands.Payments;


public sealed class RefundPaymentCommand
{
    public string PaymentId { get; init; } = string.Empty;
    public string Reason    { get; init; } = string.Empty;

    public RefundPaymentCommand() { }
}