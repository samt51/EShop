using Basket.Application.Features.BasketFeature.Queries;
using Basket.Application.Features.BasketItemFeature.Commands.UpdateBasketItem;
using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IMediator mediator;
    public BasketController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<ResponseDto<GetAllBasketQueryResponse>> GetBasketByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAllBasketQueryRequest(userId), cancellationToken);
    }

    [HttpPost]
    public async Task<ResponseDto<UpdateBasketItemCommandResponse>> UpdateItemAsync(
        UpdateBasketItemCommandRequest request, CancellationToken cancellationToken)
    {
        return await mediator.Send(request, cancellationToken);
    }
}