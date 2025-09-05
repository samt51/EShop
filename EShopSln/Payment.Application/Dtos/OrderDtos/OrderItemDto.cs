namespace Payment.Application.Dtos;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? PictureUrl { get; set; }
    public Decimal Price { get; set; }
}