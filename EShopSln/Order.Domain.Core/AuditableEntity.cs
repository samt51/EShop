namespace Order.Domain.Core;

public abstract class AuditableEntity : Entity
{
    public bool IsDeleted { get; private set; }             
    public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifyDate { get; private set; }
    public int? UserId { get; private set; }                     
    
    public void MarkCreated(int? userId = null)
    {
        CreatedDate = DateTime.UtcNow;
        UserId = userId;
    }

    public void MarkModified(int? userId = null)
    {
        ModifyDate = DateTime.UtcNow;
        UserId = userId;
    }

    public void SoftDelete(int? userId = null)
    {
        IsDeleted = true;
        ModifyDate = DateTime.UtcNow;
        UserId = userId;
    }
}
