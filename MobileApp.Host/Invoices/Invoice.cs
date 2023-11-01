namespace MobileApp.Host.Invoices;

public class Invoice : BaseModel
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
    public string Number { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
}