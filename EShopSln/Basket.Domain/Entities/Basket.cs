using EShop.Shared.Dtos.Common;

namespace Basket.Domain.Entities;

public class Basket : BaseEntity  
{
    public new string UserId { get; set; }=string.Empty;
    public List<BasketItem> basketItems { get; set; }

    public Basket()
    {
        
    }
}