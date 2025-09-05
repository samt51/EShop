using EShop.Shared.Messages.Events.CheckoutRequested;
using MassTransit;
using Order.Application.Interfaces.UnitOfWorks;

namespace Order.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IUnitOfWork _baskets; // kendi repo’n
    public OrderCreatedConsumer(IUnitOfWork baskets) => _baskets = baskets;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> ctx)
    {
  

        // İLGİLİ KULLANICININ SEPETİ TEMİZLEME
    }
}