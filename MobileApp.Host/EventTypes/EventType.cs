namespace MobileApp.Host.EventTypes;

public class EventType : BaseModel
{
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public string Name { get; set; }
    public string EmojiCode { get; set; }
    public string? Picture { get; set; }
}