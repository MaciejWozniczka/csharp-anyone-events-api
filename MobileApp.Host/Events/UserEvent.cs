using System.ComponentModel.DataAnnotations.Schema;

namespace MobileApp.Host.Events;

public class UserEvent : BaseModel
{
    public UserEvent()
    {
        IsActive = true;
    }
    public Guid EventTypeId { get; set; }
    public EventType EventType { get; set; }
    public Guid CategoryId { get; set; }
    public string CreatorId { get; set; }
    [ForeignKey("CreatorId")]
    public User Creator { get; set; }
    public List<User> UsersAssigned { get; set; }
    public DateTimeOffset EventDateTime { get; set; }
    public int Duration { get; set; }
    public Location Location { get; set; }
    public Address Address { get; set; }
    public string ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public int PeopleLimit { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    [NotMapped]
    public List<SexType>? SexTypes { get; set; }
    public bool IsActive { get; set; }
}