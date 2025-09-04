
using Basket.Application.Bases;
using Basket.Application.Dtos.BasketDtos;
using Basket.Application.Interfaces.Mapping;
using Basket.Application.Interfaces.Repositories;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Basket.Application.Features.BasketFeature.Queries;

public class GetAllBasketQueryHandler :  BaseHandler, IRequestHandler<GetAllBasketQueryRequest, ResponseDto<GetAllBasketQueryResponse>>
{
    private readonly IBasketRepository _repo;
    private readonly IMapper _mapper;
    public GetAllBasketQueryHandler(IBasketRepository repo, IMapper mapper) : base(repo, mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ResponseDto<GetAllBasketQueryResponse>> Handle(GetAllBasketQueryRequest request, CancellationToken cancellationToken)
    {
       var data =  await _repo.GetAsync(request.UserId, cancellationToken);
       var response = _mapper.Map<GetAllBasketQueryResponse,BasketResponseDto>(data.Data);
       return new ResponseDto<GetAllBasketQueryResponse>().Success(response);
    }
}