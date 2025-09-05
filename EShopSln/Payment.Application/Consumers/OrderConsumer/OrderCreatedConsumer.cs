using EShop.Shared.Messages.Events;
using MassTransit;

namespace Payment.Application.Consumers.OrderConsumer;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> ctx)
    {
        //"Ödeme ve sipariş işlemleri başarılı"
    }
}