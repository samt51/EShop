namespace Order.Application.Dtos

{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public required AddressDto Address { get; set; }

        public string BuyerId { get; set; }=string.Empty;

        public List<OrderItemDto> OrderItems { get; set; }= new();
    }
}