using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Domain.Entities;
using EShop.Shared.Messages.Events.CheckoutRequested;
using MassTransit;

namespace Catalog.Application.Consumers;

public class CheckoutRequestedConsumer : IConsumer<CheckoutRequestedEvent>
{
 private readonly IUnitOfWork _uow;

    public CheckoutRequestedConsumer(IUnitOfWork uow) => _uow = uow;

    public async Task Consume(ConsumeContext<CheckoutRequestedEvent> ctx)
    {
        var msg = ctx.Message;
        
        var productIds = msg.Items.Select(i => i.ProductId).Distinct().ToList();

        var products = await _uow.GetReadRepository<Product>()
            .GetAllAsync(p => productIds.Contains(p.Id));

        var dict = products.ToDictionary(p => p.Id);


        foreach (var i in msg.Items)
        {
            if (!dict.TryGetValue(i.ProductId, out var p))
            {
                await PublishFail(ctx, msg.BuyerId, msg.BasketId, $"Product {i.ProductId} not found");
                return;
            }

            if (p.Stock < i.Quantity)
            {
                await PublishFail(ctx, msg.BuyerId, msg.BasketId,
                    $"Insufficient stock for product {i.ProductId} (have {p.Stock}, need {i.Quantity})");
                return;
            }
        }
        
        await  _uow.OpenTransactionAsync(ctx.CancellationToken);
        try
        {
            var writeRepo = _uow.GetWriteRepository<Product>();

            foreach (var i in msg.Items)
            {
                var p = dict[i.ProductId];
                p.Stock -= i.Quantity;           
             
                await writeRepo.UpdateAsync(p);             
            }

            await _uow.SaveAsync(ctx.CancellationToken);
            await _uow.CommitAsync(ctx.CancellationToken);

            // 4) Başarı → InventoryReservedEvent
            await ctx.Publish<InventoryReservedEvent>(new
            {
                msg.BuyerId,
                msg.BasketId,
                Items = msg.Items.Select(i => new { i.ProductId, i.Quantity }).ToList()
            }, ctx.CancellationToken);
        }
        catch (Exception ex)
        {
            await _uow.RollBackAsync(ctx.CancellationToken);
            await PublishFail(ctx, msg.BuyerId, msg.BasketId, $"Reservation failed: {ex.Message}");
            throw; 
        }
    }

    private static Task PublishFail(ConsumeContext<CheckoutRequestedEvent> ctx, string buyerId, Guid basketId, string reason)
        => ctx.Publish<InventoryReservationFailedEvent>(new
        {
            BuyerId = buyerId,
            BasketId = basketId,
            Reason = reason
        }, ctx.CancellationToken);
}
