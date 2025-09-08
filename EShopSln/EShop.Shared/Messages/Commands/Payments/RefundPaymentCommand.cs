namespace EShop.Shared.Messages.Commands.Payments;


public sealed class RefundPaymentCommand
{
    public int PaymentId { get; init; } 
    public string Reason    { get; init; } = string.Empty;

    public RefundPaymentCommand() { }
}