namespace Payment.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,      // akış başladı
    Authorized = 1,   // provizyon alındı
    Captured = 2,     // tahsil edildi (tam)
    Refunded = 3,     // iade edildi (tam)
    Voided = 4,       // capture olmadan provizyon iptal
    Failed = 9        // başarısız
}