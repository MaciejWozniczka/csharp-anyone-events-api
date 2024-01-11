namespace MobileApp.Host.UserFilters;

public class UserFilter : BaseModel
{
    public UserFilter()
    {
        Location = new Location();
    }
    public string UserId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? EventTypeId { get; set; }
    public bool IsCategoryFilter { get; set; }
    public Guid LocationId { get; set; }
    public Location Location { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public SexType? SexTypes { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set;}
    public DateTime? TimeFrom { get; set;}
    public DateTime? TimeTo { get; set;}
}