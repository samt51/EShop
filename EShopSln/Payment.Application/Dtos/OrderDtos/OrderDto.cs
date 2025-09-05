
namespace Payment.Application.Dtos;



public class OrderDto
{
    public OrderDto()
    {
        OrderItems = new List<EShop.Shared.Messages.OrderItemDto>();
    }

    public int BuyerId { get; set; }

    public List<EShop.Shared.Messages.OrderItemDto> OrderItems { get; set; }

    public AddressDto Address { get; set; }
}