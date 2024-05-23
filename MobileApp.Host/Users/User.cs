using System.ComponentModel.DataAnnotations.Schema;

namespace MobileApp.Host.Users;

public class User : IdentityUser
{
    public User()
    {
        CreateDate = DateTimeOffset.UtcNow;
        IsDeleted = false;
    }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
    public Country? Country { get; set; }
    public Guid? CurrentLocationId { get; set; }
    [ForeignKey("CurrentLocationId")]
    public Location? CurrentLocation { get; set; }
    public string? Nationality { get; set; }
    public SexType? Sex { get; set; }
    public string? Picture { get; set; }
    public string? Desciption { get; set; }
    public int? PhoneNumber { get; set; }
    public string? PhoneCountryCode { get; set; }
    public UserType? UserType { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletingDate { get; set; }
    [InverseProperty("Creator")]
    public List<UserEvent>? EventsCreated { get; set; }
    public List<UserEvent>? EventsCooperationPending { get; set; }
    public List<UserEvent>? EventsCooperated { get; set; }
    public List<UserEvent>? EventsPending { get; set; }
    public List<UserEvent>? EventsAssigned { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
}