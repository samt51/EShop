using Catalog.Application.Bases;
using Catalog.Application.Interfaces.Mapping;
using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Domain.Entities;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Catalog.Application.Features.CategoryFeature.Queries.GetAllCategory;

public class GetAllCategoryQueryHandler : BaseHandler, IRequestHandler<GetAllCategoryQueryRequest, ResponseDto<IList<GetAllCategoryResponse>>>
{
    public GetAllCategoryQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<IList<GetAllCategoryResponse>>> Handle(GetAllCategoryQueryRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Category>().GetAllAsync();
        
        var map = mapper.Map<GetAllCategoryResponse,Category>(data);

        return new ResponseDto<IList<GetAllCategoryResponse>>().Success(map);
    }
}