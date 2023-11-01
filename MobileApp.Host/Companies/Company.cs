namespace MobileApp.Host.Companies;

public class Company : BaseModel
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
    public Guid AddressId { get; set; }
    public Address Address { get; set; }
    public string BankAccount { get; set; }
    public string Picture { get; set; }
}