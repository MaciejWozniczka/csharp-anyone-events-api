using MobileApp.Host.Infrastructure;

namespace MobileApp.Host.Users;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public Countries Country { get; set; }
    public string City { get; set; }
    public string Nationality { get; set; }
    public SexType Sex { get; set; }
    public List<string> Languages { get; set; }
    public Guid? PictureId { get; set; }
    public string Desciption { get; set; }
    public int PhoneNumber { get; set; }
    public string PhoneCountryCode { get; set; }
}