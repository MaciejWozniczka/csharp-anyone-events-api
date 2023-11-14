using MobileApp.Host.Locations;

namespace MobileApp.Host.Users;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public Country Country { get; set; }
    public Location Location { get; set; }
    public string Nationality { get; set; }
    public SexType Sex { get; set; }
    public List<string> Languages { get; set; }
    public string Picture { get; set; }
    public string Desciption { get; set; }
    public int PhoneNumber { get; set; }
    public string PhoneCountryCode { get; set; }
    public UserType UserType { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DeletingDate { get; set; }
    public List<Event> EventsCreated { get; set; }
    public List<Event> EventsAssigned { get; set; }
}