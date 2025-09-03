using Catalog.Application.Bases;
using Catalog.Application.Interfaces.Mapping;
using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Domain.Entities;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Catalog.Application.Features.ProductFeature.Commands;

public class CreateProductCommandHandler :  BaseHandler, IRequestHandler<CreateProductCommandRequest, ResponseDto<CreateProductCommandResponse>>
{
    public CreateProductCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<CreateProductCommandResponse>> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
    {
        var map = mapper.Map<Product, CreateProductCommandRequest>(request);
        
        unitOfWork.OpenTransaction();
        
        await unitOfWork.GetWriteRepository<Product>().AddAsync(map, cancellationToken);

        await unitOfWork.SaveAsync();

        await unitOfWork.CommitAsync();
        
        return new ResponseDto<CreateProductCommandResponse>().Success();   
    }
}