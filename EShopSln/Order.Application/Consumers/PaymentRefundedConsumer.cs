using EShop.Shared.Enums;
using EShop.Shared.Messages.Events;
using MassTransit;
using Order.Application.Interfaces.UnitOfWorks;

namespace Order.Application.Consumers;

public class PaymentRefundedConsumer : IConsumer<PaymentRefundedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentRefundedConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<PaymentRefundedEvent> ctx)
    {
        var message = ctx.Message;
        var data = await _unitOfWork.GetReadRepository<Domain.OrderAggregate.Order>()
            .GetAsync(s => s.Id == message.OrderId);

        data.UpdateStatus(OrderStatus.Refunded);

        await _unitOfWork.OpenTransactionAsync(ctx.CancellationToken);

        await _unitOfWork.GetWriteRepository<Domain.OrderAggregate.Order>().AddAsync(data);
        
        await _unitOfWork.SaveAsync(ctx.CancellationToken);
        
        await _unitOfWork.CommitAsync(ctx.CancellationToken);   
        
    }
}