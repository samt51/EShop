using Catalog.Application.Bases;
using Catalog.Application.Interfaces.Mapping;
using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Domain.Entities;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace Catalog.Application.Features.CategoryFeature.Commands;

public class CreateCategoryCommandHandler :  BaseHandler, IRequestHandler<CreateCategoryCommandRequest, ResponseDto<CreateCategoryCommandResponse>>
{
    private readonly IOutputCacheStore _outputCache;
    public CreateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork,IOutputCacheStore outputCache) : base(mapper, unitOfWork)
    {
        _outputCache = outputCache;
    }

    public async Task<ResponseDto<CreateCategoryCommandResponse>> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var data = mapper.Map<Category, CreateCategoryCommandRequest>(request);
        
        await unitOfWork.OpenTransactionAsync(cancellationToken);
        
        await unitOfWork.GetWriteRepository<Category>().AddAsync(data, cancellationToken);

       await unitOfWork.SaveAsync(cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);
        
        await _outputCache.EvictByTagAsync("departments", cancellationToken);
        
        return new ResponseDto<CreateCategoryCommandResponse>().Success();

    }
}