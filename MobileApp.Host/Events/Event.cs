namespace MobileApp.Host.Events;

public class Event : BaseModel
{
    public Event()
    {
        IsActive = true;
    }
    public Guid EventTypeId { get; set; }
    public EventType EventType { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public string CreatorId { get; set; }
    public List<User> UsersAssigned { get; set; }
    public string Name { get; set; }
    public DateTime EventDateTime { get; set; }
    public int Duration { get; set; }
    public Address Location { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public string? Picture { get; set; }
    public int PeopleLimit { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public List<string>? Cities { get; set; }
    public List<Country>? Countries { get; set; }
    public List<string>? Nationalities { get; set; }
    public List<SexType>? SexTypes { get; set; }
    public List<string>? Languages { get; set; }
    public List<ExperienceLevel>? ExperienceLevels { get; set; }
    public TripType? TripType { get; set; }
    public string? Budget { get; set; }
    public string? Destination { get; set; }
    public bool IsActive { get; set; }
}