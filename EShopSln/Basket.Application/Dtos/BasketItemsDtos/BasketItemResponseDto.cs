namespace Basket.Application.Dtos.BasketItemsDtos;

public class BasketItemResponseDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } =string.Empty;
    public string? ImageUrl { get; set; }
    public int BasketId { get; set; }
}