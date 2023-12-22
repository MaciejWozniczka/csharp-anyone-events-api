namespace MobileApp.Host.Communications;

public class Communication : BaseModel
{
    public Guid? EventId { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
}