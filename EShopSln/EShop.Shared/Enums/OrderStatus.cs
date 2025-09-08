namespace EShop.Shared.Enums;

public enum OrderStatus
{
    Pending = 0,          // Sipariş oluşturuldu
    AwaitingPayment = 1,  // Stok ayrıldı, ödeme bekleniyor
    Paid = 2,             // Ödeme onaylandı
    Completed = 3,        // Teslim/sürecin tamamı bitti (opsiyonel)
    Canceled = 4,         // İptal
    Refunded = 5          // İade Edildi
}