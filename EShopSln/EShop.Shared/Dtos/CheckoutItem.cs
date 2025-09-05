namespace EShop.Shared.Dtos;

public interface CheckoutItem
{
  public  int ProductId { get; init; }
  public  int Quantity { get; init; }
  public decimal UnitPrice { get; init; }
}