namespace EShop.Shared.Dtos.Common;

public class BaseEntity : IBaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifyDate { get; set; }
    public int? UserId { get; set; }
}