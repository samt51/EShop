using Catalog.Application.Bases;
using Catalog.Application.Interfaces.Mapping;
using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Domain.Entities;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Catalog.Application.Features.CategoryFeature.Commands;

public class CreateCategoryCommandHandler :  BaseHandler, IRequestHandler<CreateCategoryCommandRequest, ResponseDto<CreateCategoryCommandResponse>>
{
    public CreateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<CreateCategoryCommandResponse>> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var data = mapper.Map<Category, CreateCategoryCommandRequest>(request);
        
        unitOfWork.OpenTransaction();
        
        await unitOfWork.GetWriteRepository<Category>().AddAsync(data, cancellationToken);

        await unitOfWork.SaveAsync();

        await unitOfWork.CommitAsync();

        return new ResponseDto<CreateCategoryCommandResponse>().Success();

    }
}