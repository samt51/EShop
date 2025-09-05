namespace EShop.Shared.Dtos;

public sealed class OrderItemDto
{
    public int ProductId { get; init; }         
    public string ProductName { get; init; } = "";
    public decimal Price { get; init; }
    public string? PictureUrl { get; init; }
}