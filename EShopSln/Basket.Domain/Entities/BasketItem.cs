using EShop.Shared.Dtos.Common;

namespace Basket.Domain.Entities;

public class BasketItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } =string.Empty;
    public string? ImageUrl { get; set; }
    public int BasketId { get; set; }
    public required Basket Basket { get; set; }= default!;

    public BasketItem(int id,int quantity, decimal price, int productId, string productName, string? imageUrl, int basketId)
    {
        this.Id = id;
        this.Quantity = quantity;
        this.Price = price;
        this.ProductId = productId;
        this.ProductName = productName;
        this.ImageUrl = imageUrl;
        this.BasketId = basketId;
    }

    public BasketItem()
    {
        
    }
}