using AutoMapper.Internal.Mappers;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Bases;
using Order.Application.Dtos;
using Order.Application.Interfaces.Mapping;
using Order.Application.Interfaces.UnitOfWorks;

namespace Order.Application.Features.OrderFeature.Queries.GetOrderByUserId;

public class GetOrderByUserIdQueryHandler:  BaseHandler,IRequestHandler<GetOrderByUserIdQueryRequest, ResponseDto<List<GetOrderByUserIdQueryResponse>>>
{
    public GetOrderByUserIdQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<List<GetOrderByUserIdQueryResponse>>> Handle(GetOrderByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.GetReadRepository<Domain.OrderAggregate.Order>().GetAllAsync(x=>x.BuyerId==request.UserId
        ,include:x=>x.Include(y=>y.OrderItems));
        var map = mapper.Map<GetOrderByUserIdQueryResponse,Domain.OrderAggregate.Order>(order);
      
        return new ResponseDto<List<GetOrderByUserIdQueryResponse>>().Success(map.ToList());
    }
}