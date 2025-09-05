using EShop.Shared.Messages;
using EShop.Shared.Messages.Commands.Payments;
using EShop.Shared.Messages.Events;
using MassTransit;
using Order.Application.Interfaces.UnitOfWorks;
using Serilog;
using ArgumentException = System.ArgumentException;

namespace Order.Application.Consumers
{
    public class PaymentAuthorizedConsumer : IConsumer<PaymentAuthorizedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentAuthorizedConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<PaymentAuthorizedEvent> context)
        {
            var msg = context.Message;

            if (string.IsNullOrWhiteSpace(msg.BuyerId) || msg.OrderItems is null || msg.OrderItems.Count == 0)
                throw new ArgumentException("Invalid order payload");

            await _unitOfWork.OpenTransactionAsync(context.CancellationToken);

            try
            {
                var address = new Domain.OrderAggregate.Address(
                    msg.Province, msg.District, msg.Street, msg.ZipCode, msg.Line);

                var order = new Domain.OrderAggregate.Order(
                    Convert.ToInt32(msg.BuyerId), address);

                foreach (var i in msg.OrderItems)
                {
                    if (i.Price <= 0) throw new ArgumentException("Item price must be > 0");
                    order.AddOrderItem(i.ProductId, i.ProductName, i.Price, i.PictureUrl);
                }

                await _unitOfWork.GetWriteRepository<Domain.OrderAggregate.Order>()
                                 .AddAsync(order, context.CancellationToken);

                await _unitOfWork.SaveAsync(context.CancellationToken);
                await _unitOfWork.CommitAsync(context.CancellationToken);
             

                await context.Publish<OrderCreatedEvent>(new
                {
                    CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
                    OrderId = order.Id,
                    BuyerId = order.BuyerId,
                    Total   = order.GetTotalPrice   // property'in adı sende böyle
                }, context.CancellationToken);

                Log.Information("Order {OrderId} created for {Buyer}", order.Id, msg.BuyerId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync(context.CancellationToken);
                Log.Error(ex, "Order creation failed for Buyer {BuyerId}", msg.BuyerId);

           
                await context.Send<RefundPaymentCommand>(new
                {
                    CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
                    PaymentId = msg.PaymentId,   
                    Reason = "OrderCreationFailed"
                }, context.CancellationToken);

                throw; 
            }
        }
    }
}
