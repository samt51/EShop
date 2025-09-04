using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Order.Application.Features.OrderFeature.Queries.GetOrderByUserId;

public class GetOrderByUserIdQueryRequest :  IRequest<ResponseDto<List<GetOrderByUserIdQueryResponse>>>
{
    public int UserId { get; set; }

    public GetOrderByUserIdQueryRequest(int userId)
    {
        this.UserId = userId;
    }
}