namespace MobileApp.Host.Models;

public class BaseModel
{
    public BaseModel()
    {
        CreateDate = DateTime.UtcNow;
        IsDeleted = false;
    }
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DeletingDate { get; set; }
}