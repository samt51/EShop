using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Basket.Application.Features.BasketItemFeature.Commands.UpdateBasketItem;

public class UpdateBasketItemCommandRequest :  IRequest<ResponseDto<UpdateBasketItemCommandResponse>>
{
    public string UserId { get; set; } =  string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } =string.Empty;
    public string? ImageUrl { get; set; }
    public int BasketId { get; set; }
}