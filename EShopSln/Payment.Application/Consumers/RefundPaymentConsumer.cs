using EShop.Shared.Messages.Commands.Payments;
using EShop.Shared.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Interfaces.UnitOfWorks;
using Payment.Domain.Enums;

namespace Payment.Application.Consumers;

public class RefundPaymentConsumer : IConsumer<RefundPaymentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefundPaymentConsumer> _logger;

    public RefundPaymentConsumer(IUnitOfWork unitOfWork, ILogger<RefundPaymentConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RefundPaymentCommand> ctx)
    {
        var m = ctx.Message;

        var payment = await _unitOfWork
            .GetReadRepository<Payment.Domain.Entities.Payment>()
            .GetAsync(x => x.Id == m.PaymentId, ctx: ctx.CancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Refund ignored: Payment not found. PaymentId={PaymentId} Corr={CorrelationId}",
                m.PaymentId, ctx.CorrelationId);
            return;
        }

        // zaten iade/iptal edilmişse
        if (payment.Status is PaymentStatus.Refunded or PaymentStatus.Voided)
        {
            _logger.LogInformation("Refund/void already done. PaymentId={PaymentId} Status={Status} Corr={CorrelationId}",
                payment.Id, payment.Status, ctx.CorrelationId);
            return;
        }

        var actionTaken = false;

        if (payment.Status == PaymentStatus.Authorized)
        {
       
            payment.MarkVoided(m.Reason);
            actionTaken = true;
            
            await ctx.Publish(new PaymentVoidedEvent(
                ctx.CorrelationId ?? Guid.NewGuid(),
                payment.OrderId,
                payment.Id,
                DateTime.UtcNow
            ), ctx.CancellationToken);

            _logger.LogInformation("Authorization voided. PaymentId={PaymentId} Corr={CorrelationId}",
                payment.Id, ctx.CorrelationId);
        }
        else if (payment.Status == PaymentStatus.Captured)
        {
          
            payment.MarkRefunded(m.Reason);
            actionTaken = true;

            await ctx.Publish(new PaymentRefundedEvent(
                ctx.CorrelationId ?? Guid.NewGuid(),
                payment.OrderId,
                payment.Id,
                payment.Amount,         
                DateTime.UtcNow
            ), ctx.CancellationToken);
            
            _logger.LogInformation("Captured amount refunded. PaymentId={PaymentId} Corr={CorrelationId}",
                payment.Id, ctx.CorrelationId);
        }
        else
        {
            _logger.LogWarning(
                "Refund ignored due to invalid state. PaymentId={PaymentId} CurrentStatus={Status} Corr={CorrelationId}",
                payment.Id, payment.Status, ctx.CorrelationId);
            return;
        }

        if (!actionTaken) return;

        await _unitOfWork.OpenTransactionAsync(ctx.CancellationToken);
        try
        {
            await _unitOfWork.GetWriteRepository<Payment.Domain.Entities.Payment>()
                .UpdateAsync(payment);

            await _unitOfWork.SaveAsync(ctx.CancellationToken);
            await _unitOfWork.CommitAsync(ctx.CancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollBackAsync(ctx.CancellationToken);
            _logger.LogError(ex, "Refund/void transaction failed. PaymentId={PaymentId} Corr={CorrelationId}",
                payment.Id, ctx.CorrelationId);
            throw;
        }
    }
}