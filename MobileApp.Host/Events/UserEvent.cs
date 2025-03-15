namespace MobileApp.Host.Events;

public class UserEvent : BaseModel
{
    public UserEvent()
    {
        IsActive = true;
        Cooperators = new List<User>();
        CooperatorsPending = new List<User>();
        UsersPending = new List<User>();
        GroupsPending = new List<UserGroup>();
        UsersAssigned = new List<User>();
        UsersInterested = new List<User>();
        UsersSkipped = new List<User>();
        SexTypes = new List<SexType>();
    }
    public Guid EventTypeId { get; set; }
    public EventType EventType { get; set; }
    public Guid CategoryId { get; set; }
    public string CreatorId { get; set; }
    public User Creator { get; set; }
    public List<User>? Cooperators { get; set; }
    public List<User>? CooperatorsPending { get; set; }
    public List<User>? UsersPending { get; set; }
    public List<UserGroup>? GroupsPending { get; set; }
    public List<User>? UsersAssigned { get; set; }
    public List<User>? UsersInterested { get; set; }
    public List<User>? UsersSkipped { get; set; }
    public DateTimeOffset EventDateTime { get; set; }
    public int Duration { get; set; }
    public Guid LocationId { get; set; }
    public Location Location { get; set; }
    public Guid AddressId { get; set; }
    public Address Address { get; set; }
    public string ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public int PeopleLimit { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public List<SexType>? SexTypes { get; set; }
    public bool IsActive { get; set; }
}