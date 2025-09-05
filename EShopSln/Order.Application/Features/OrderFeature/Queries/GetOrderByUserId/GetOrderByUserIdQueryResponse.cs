using Order.Application.Dtos;

namespace Order.Application.Features.OrderFeature.Queries.GetOrderByUserId;

public class GetOrderByUserIdQueryResponse
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }

    public AddressDto? Address { get; set; }

    public string? BuyerId { get; set; }

    public List<OrderItemDto> OrderItems { get; set; }= new();
}