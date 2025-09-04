using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.OrderFeature.Commands.CreateOrder;
using Order.Application.Features.OrderFeature.Queries.GetOrderByUserId;


namespace Order.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
     

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ResponseDto<List<GetOrderByUserIdQueryResponse>>> GetOrders(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetOrderByUserIdQueryRequest(1), cancellationToken);
        }

        [HttpPost]
        public async Task<ResponseDto<CreateOrderCommandResponse>> SaveOrderAsnyc(CreateOrderCommandRequest createOrderCommand, CancellationToken cancellationToken)
        {
            return await _mediator.Send(createOrderCommand, cancellationToken);
        }
    }
}