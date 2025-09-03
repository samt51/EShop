using EShop.Shared.Dtos.Common;

namespace Catalog.Domain.Entities;

public class Category : BaseEntity  
{
    public string Name { get; set; }
    public IList<Product> Products { get; set; }

    public Category(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
}