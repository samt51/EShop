namespace EShop.Shared.Messages.Commands.Payments;

public class RefundPaymentCommand
{
    public Guid CorrelationId { get; init; }
   public string PaymentId { get; init; }   
   public string Reason { get; init; }
}