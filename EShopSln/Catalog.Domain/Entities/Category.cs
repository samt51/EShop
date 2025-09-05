using EShop.Shared.Dtos.Common;

namespace Catalog.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public List<Product> Products { get; set; }=new();

    public Category(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public Category()
    {
      
    }
}