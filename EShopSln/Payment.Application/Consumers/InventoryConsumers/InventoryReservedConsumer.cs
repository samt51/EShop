using EShop.Shared.Messages.Events;
using EShop.Shared.Messages.Events.CheckoutRequested;
using MassTransit;

namespace Payment.Application.Consumers.InventoryConsumers;

public class InventoryReservedConsumer : IConsumer<InventoryReservedEvent>
{
    

    public async Task Consume(ConsumeContext<InventoryReservedEvent> ctx)
    {
        // burada stock var olduktan sonra kart ile ödencek tutar bloklanır rezerve edilir.
       

        //if (auth.Success)
        { //olumlu ise ödeme tarafına geçer.
            await ctx.Publish<PaymentAuthorizedEvent>(new
            {
                CorrelationId = Guid.NewGuid(),
                BuyerId = ctx.Message.BuyerId,
                //PaymentId = auth.PaymentId,
                OrderItems = ctx.Message.Items.Select(i => new {
                    ProductId = i.ProductId,
                    ProductName = "",      
                    Price = 0m,           
                    PictureUrl = (string?)null
                }).ToList()
            }, ctx.CancellationToken);
        }
       // else
        {
            // olumsuz ise buradaki event tetiklenir.
            await ctx.Publish<PaymentFailedEvent>(new
            {
                CorrelationId = Guid.NewGuid(),
                BuyerId = ctx.Message.BuyerId,
                //Reason = auth.ErrorMessage ?? "Authorize failed"
            }, ctx.CancellationToken);

            // stoğu geri bırak
            await ctx.Publish<InventoryReservationReleaseRequestedEvent>(new
            {
                BuyerId = ctx.Message.BuyerId,
                BasketId = ctx.Message.BasketId
            }, ctx.CancellationToken);
        }
    }
}