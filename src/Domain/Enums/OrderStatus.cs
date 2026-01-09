namespace Domain.Enums;

public enum OrderStatus
{
	Pending = 1,      // Sipariş oluşturuldu
	Paid = 2,         // Ödeme alındı
	Preparing = 3,    // Hazırlanıyor
	Shipped = 4,      // Kargoya verildi
	Delivered = 5,    // Teslim edildi
	Cancelled = 6     // İptal edildi
}
