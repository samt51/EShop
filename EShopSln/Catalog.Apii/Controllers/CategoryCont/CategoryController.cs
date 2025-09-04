using Catalog.Application.Features.CategoryFeature.Commands;
using Catalog.Application.Features.CategoryFeature.Queries.GetAllCategory;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Catalog.Apii.Controllers.CategoryCont;

[Route("api/[controller]/[action]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IMediator mediator;

    public CategoryController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    [HttpPost]
    public async Task<ResponseDto<CreateCategoryCommandResponse>> AddAsync(CreateCategoryCommandRequest request,CancellationToken cancellationToken)
    {
        return await mediator.Send(request,cancellationToken);
    }
    
    [HttpGet]
    [OutputCache(PolicyName = "Departments", Tags = new[] { "departments" })]
    public async Task<ResponseDto<IList<GetAllCategoryResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAllCategoryQueryRequest(), cancellationToken);
    }
}