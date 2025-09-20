namespace AnyOneApi.Host.Models;

public class BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreateDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletingDate { get; set; }
}