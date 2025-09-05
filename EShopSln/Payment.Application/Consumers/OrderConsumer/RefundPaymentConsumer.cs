using EShop.Shared.Messages.Commands.Payments;
using MassTransit;

namespace Payment.Application.Consumers.OrderConsumer;

public class RefundPaymentConsumer : IConsumer<RefundPaymentCommand>
{
    public async Task Consume(ConsumeContext<RefundPaymentCommand> ctx)
    {
        // istersen PaymentRefundedEvent yayınla
    }
}