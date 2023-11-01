namespace MobileApp.Host.Models;

public class BaseModel
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DeletingDate { get; set; }
}