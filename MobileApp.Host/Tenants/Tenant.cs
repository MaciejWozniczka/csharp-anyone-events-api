namespace MobileApp.Host.Tenants;

public class Tenant : BaseModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Industry { get; set; }
    public string TaxNumber { get; set; }
    public string Logo { get; set; }
    public Guid AddressId { get; set; }
    public Address Address { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public List<Company> Companies { get; set; }
}