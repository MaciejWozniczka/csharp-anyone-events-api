namespace MobileApp.Host.Addresses;

public class Address : BaseModel
{
    public Country Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Street { get; set; }
    public string StreeNumber { get; set; }
    public string ApartmentNumber { get; set; }
}