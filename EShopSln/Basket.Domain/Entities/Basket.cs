using EShop.Shared.Dtos.Common;

namespace Basket.Domain.Entities;

public class Basket : BaseEntity  
{
    public int UserId { get; set; }
    public List<BasketItem> basketItems { get; set; }

    public Basket(int id,int userId, List<BasketItem> basketItems)
    {
        this.Id = id;
        this.UserId = userId;
        this.basketItems = basketItems;
    }

    public Basket()
    {
        
    }
}