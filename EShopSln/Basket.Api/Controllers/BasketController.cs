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
  
}