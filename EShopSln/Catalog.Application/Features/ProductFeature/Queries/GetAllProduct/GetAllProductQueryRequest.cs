using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Catalog.Application.Features.ProductFeature.Queries.GetAllProduct;

public class GetAllProductQueryRequest : IRequest<ResponseDto<IList<GetAllProductQueryResponse>>>
{
    
}