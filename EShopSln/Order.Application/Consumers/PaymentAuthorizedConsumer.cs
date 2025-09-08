using EShop.Shared.Enums;
using EShop.Shared.Messages.Commands.Payments;
using EShop.Shared.Messages.Events;
using MassTransit;
using Order.Application.Interfaces.UnitOfWorks;

namespace Order.Application.Consumers;

public class PaymentAuthorizedConsumer : IConsumer<PaymentAuthorizedEvent>
{
    private readonly IUnitOfWork _uow;

    public PaymentAuthorizedConsumer(IUnitOfWork uow) => _uow = uow;

    public async Task Consume(ConsumeContext<PaymentAuthorizedEvent> context)
    {
        var m = context.Message;


        if (m.OrderId == 0)
            throw new ArgumentException("Sipariş Nosu eksik");
        if (m.BuyerId == 0)
            throw new ArgumentException("Alıcı Bilgisi eskik");


        var correlationId = m.CorrelationId != Guid.Empty
            ? m.CorrelationId
            : (context.CorrelationId ?? Guid.NewGuid());

        await _uow.OpenTransactionAsync(context.CancellationToken);
        try
        {
            var orderRepo = _uow.GetReadRepository<Domain.OrderAggregate.Order>();
            var order = await orderRepo.GetAsync(x => x.Id == m.OrderId, ct: context.CancellationToken);
            if (order is null)
                throw new InvalidOperationException($"Sipariş Bulunamadı: {m.OrderId}");


            if (order.Status is OrderStatus.Paid
                or OrderStatus.Completed)
            {
                await _uow.RollBackAsync(context.CancellationToken);
                return;
            }

            order.MarkAsPaid(m.OccurredOnUtc);

            await _uow.SaveAsync(context.CancellationToken);
            await _uow.CommitAsync(context.CancellationToken);

            // bu event ödeme alınan siparişlerin bir sonraki süreç (paketleme veya mail gibi)
            // servislerde consume edilir.
            await context.Publish(new OrderPaidEvent(
                CorrelationId: correlationId,
                OrderId: order.Id,
                PaidAtUtc: order.PaidAtUtc ?? m.OccurredOnUtc,
                BuyerId: order.BuyerId ?? 0,
                Total: order.GetTotalPrice
            ), context.CancellationToken);
        }
        catch
        {
            await _uow.RollBackAsync(context.CancellationToken);

            // Telafi (compensation) — Refund
            await context.Send<RefundPaymentCommand>(new
            {
                CorrelationId = correlationId,
                PaymentId = m.PaymentId,
                Reason = "OrderUpdateFailed"
            }, context.CancellationToken);
        }
    }
}
