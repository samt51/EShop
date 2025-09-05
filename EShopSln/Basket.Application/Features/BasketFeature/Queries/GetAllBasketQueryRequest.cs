using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Basket.Application.Features.BasketFeature.Queries;

public class GetAllBasketQueryRequest :  IRequest<ResponseDto<GetAllBasketQueryResponse>>
{
    public string UserId { get; set; } =  string.Empty;

    public GetAllBasketQueryRequest(string userId)
    {
        this.UserId = userId;   
    }
}