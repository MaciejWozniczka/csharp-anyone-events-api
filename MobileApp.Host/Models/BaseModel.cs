namespace MobileApp.Host.Models;

public class BaseModel
{
    public BaseModel()
    {
        Id = Guid.NewGuid();
        CreateDate = DateTime.UtcNow;
        IsDeleted = false;
    }
    public Guid Id { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset DeletingDate { get; set; }
}