using Catalog.Application.Bases;
using Catalog.Application.Dtos.CategoryDto;
using Catalog.Application.Interfaces.Mapping;
using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Domain.Entities;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ProductFeature.Queries.GetAllProduct;

public class GetAllProductQueryHandler :  BaseHandler, IRequestHandler<GetAllProductQueryRequest, ResponseDto<IList<GetAllProductQueryResponse>>>
{
    public GetAllProductQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<IList<GetAllProductQueryResponse>>> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
    {
        var products = await unitOfWork
            .GetReadRepository<Product>()
            .GetAllAsync(x=>!x.IsDeleted,x=>x.Include(y=>y.Category));

        var dtoList = mapper.Map<GetAllProductQueryResponse, Product>(products);

        return new ResponseDto<IList<GetAllProductQueryResponse>>().Success(dtoList);
    }
}