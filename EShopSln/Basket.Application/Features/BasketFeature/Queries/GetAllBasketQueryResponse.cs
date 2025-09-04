using Basket.Application.Dtos.BasketItemsDtos;

namespace Basket.Application.Features.BasketFeature.Queries;

public class GetAllBasketQueryResponse
{
    public int UserId { get; set; }
    public List<BasketItemResponseDto> basketItems { get; set; }
}