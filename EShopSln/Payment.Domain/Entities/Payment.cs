using EShop.Shared.Dtos.Common;
using Payment.Domain.Enums;

namespace Payment.Domain.Entities;

public class Payment:BaseEntity
{
    public int OrderId { get; private set; }
    public int BuyerId { get; private set; }
    public Guid CorrelationId { get; private set; }
    
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "TRY";
    
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    

    public string? Gateway { get; private set; }
    public string? GatewayPaymentId { get; private set; }
    public string Reason { get; set; }   
    
    public Payment() { }
    
    
    public Payment(int orderId, int buyerId, Guid correlationId, decimal amount, string currency = "TRY", string? gateway = null)
    {
        if (orderId <= 0) throw new ArgumentException(nameof(orderId));
        if (buyerId <= 0) throw new ArgumentException(nameof(buyerId));
        if (correlationId == Guid.Empty) throw new ArgumentException(nameof(correlationId));
        if (amount <= 0) throw new ArgumentException(nameof(amount));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException(nameof(currency));

        OrderId = orderId;
        BuyerId = buyerId;
        CorrelationId = correlationId;
        Amount = amount;
        Currency = currency;
        Gateway = gateway;
    }
    
    // Basit domain metotları
    public void MarkAuthorized(string? gatewayPaymentId = null)
    {
        Status = PaymentStatus.Authorized;
        if (!string.IsNullOrWhiteSpace(gatewayPaymentId))
            GatewayPaymentId = gatewayPaymentId;
    }

    public void MarkCaptured()
        => Status = PaymentStatus.Captured;

    public void MarkRefunded(string reason)
    {
        Status = PaymentStatus.Refunded;
        Reason = reason;
    }


    public void MarkVoided(string reason)
    {
        Status = PaymentStatus.Voided;
        Reason = reason;
    }
    

    public void Fail() => Status = PaymentStatus.Failed;

}