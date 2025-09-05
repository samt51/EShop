using Basket.Application.Dtos.BasketItemsDtos;

namespace Basket.Application.Dtos.BasketDtos;

public class BasketResponseDto
{
    public int Id { get; set; }
    public string UserId { get; set; } =  string.Empty;
    public List<BasketItemResponseDto> basketItems { get; set; } = new();
}