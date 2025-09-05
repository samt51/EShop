

namespace EShop.Shared.Messages
{
    public sealed class CreateOrderMessageCommand
    {
        // Adres alanları
        public string Province { get; init; } = string.Empty;
        public string District { get; init; } = string.Empty;
        public string Street   { get; init; } = string.Empty;
        public string ZipCode  { get; init; } = string.Empty;
        public string Line     { get; init; } = string.Empty;

        // Diğer alanlar…
        public string BuyerId  { get; init; } = string.Empty;

        public List<CreateOrderItem> Items { get; init; } = new();

        public CreateOrderMessageCommand() { }
    }

    
    public sealed class CreateOrderItem
    {
        public int    ProductId   { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal Price      { get; init; }
        public string PictureUrl  { get; init; } = string.Empty;
        public int    Count       { get; init; }
    }
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }
    }
}