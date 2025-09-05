using Basket.Application.Bases;
using Basket.Application.Dtos.BasketDtos;
using Basket.Application.Dtos.BasketItemsDtos;
using Basket.Application.Interfaces.Mapping;
using Basket.Application.Interfaces.Repositories;
using EShop.Shared.Dtos.BasesResponses;
using EShop.Shared.Messages.Events.CheckoutRequested;
using MassTransit;
using MediatR;

namespace Basket.Application.Features.BasketItemFeature.Commands.UpdateBasketItem;

public class UpdateBasketItemCommandHandler(IBasketRepository repo, IMapper mapper, IPublishEndpoint publish) : BaseHandler(repo, mapper),
    IRequestHandler<UpdateBasketItemCommandRequest, ResponseDto<UpdateBasketItemCommandResponse>>
{
    private readonly IPublishEndpoint _publish = publish;
    public async Task<ResponseDto<UpdateBasketItemCommandResponse>> Handle(UpdateBasketItemCommandRequest request, CancellationToken cancellationToken)
    {
        // satın al tetiklendiğin de 
        var data = new BasketResponseDto { UserId = request.UserId };
        var basket = await repo.GetAsync(request.UserId.ToString(), cancellationToken) ?? new ResponseDto<BasketResponseDto>(){Data = data} ;
        var item = basket.Data.basketItems.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (item is null)
        {
            item = new BasketItemResponseDto
            {
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                Quantity = request.Quantity,
                ImageUrl = request.ImageUrl
            };
            basket.Data.basketItems.Add(item);
        }
        else
        {
            item.Quantity += request.Quantity;
        }

        var map = mapper.Map<Domain.Entities.Basket, BasketResponseDto>(basket.Data);
        await _publish.Publish<CheckoutRequestedEvent>(new
        {
            BuyerId =request.UserId,
            BasketId = basket.Data.Id,
            Items = basket.Data.basketItems.Select(i => new { i.ProductId, i.Quantity,i.Price }).ToList()
        }, cancellationToken);

        await repo.UpsertAsync(map, ct: cancellationToken);
        
        return new ResponseDto<UpdateBasketItemCommandResponse>().Success();
    }
}