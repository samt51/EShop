
using EShop.Shared.Dtos.Common;

namespace Catalog.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}