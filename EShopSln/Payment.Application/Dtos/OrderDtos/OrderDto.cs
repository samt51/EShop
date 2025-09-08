
namespace Payment.Application.Dtos;



public class OrderDto
{
    public OrderDto()
    {
        OrderItems = new List<EShop.Shared.Dtos.OrderItemDto>();
    }

    public int OrderId { get; set; }
    public int BuyerId { get; set; }

    public List<EShop.Shared.Dtos.OrderItemDto> OrderItems { get; set; }

    public AddressDto Address { get; set; }
}