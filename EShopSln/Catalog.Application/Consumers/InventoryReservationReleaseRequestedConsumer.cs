using Catalog.Application.Interfaces.UnitOfWorks;
using EShop.Shared.Messages.Events.CheckoutRequested;
using MassTransit;

namespace Catalog.Application.Consumers;

public class InventoryReservationReleaseRequestedConsumer : IConsumer<InventoryReservationReleaseRequestedEvent>
{
    private readonly IUnitOfWork _uow;
    public InventoryReservationReleaseRequestedConsumer(IUnitOfWork uow) => _uow = uow;

    public async Task Consume(ConsumeContext<InventoryReservationReleaseRequestedEvent> ctx)
    {
       //ilgili ürünü rezarvasyondan çıkarma
    }
}