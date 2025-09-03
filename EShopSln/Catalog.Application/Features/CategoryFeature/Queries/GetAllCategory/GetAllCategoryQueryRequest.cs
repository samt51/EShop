using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Catalog.Application.Features.CategoryFeature.Queries.GetAllCategory;

public class GetAllCategoryQueryRequest : IRequest<ResponseDto<IList<GetAllCategoryResponse>>>
{
    public GetAllCategoryQueryRequest()
    {
        
    }
}