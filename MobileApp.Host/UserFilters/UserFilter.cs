namespace MobileApp.Host.UserFilters;

public class UserFilter : BaseModel
{
    public string UserId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? EventTypeId { get; set; }
    public bool IsCategoryFilter { get; set; }
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; } = new();
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public SexType? SexTypes { get; set; }
    public DateTime? DateTimeFrom { get; set; }
    public DateTime? DateTimeTo { get; set;}
}