using Catalog.Application.Features.ProductFeature.Commands;
using Catalog.Application.Features.ProductFeature.Queries.GetAllProduct;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.ProductCont;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator mediator;

    public ProductController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    [HttpPost]
    public async Task<ResponseDto<CreateProductCommandResponse>> AddAsync(CreateProductCommandRequest request,CancellationToken cancellationToken)
    {
        return await mediator.Send(request,cancellationToken);
    }
    
    [HttpGet]
    public async Task<ResponseDto<IList<GetAllProductQueryResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAllProductQueryRequest(), cancellationToken);
    }
}