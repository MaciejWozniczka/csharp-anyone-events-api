namespace MobileApp.Host.Categories;

public class Category : BaseModel
{
    public Category()
    {
        EventTypes = [];
    }
    public string Name { get; set; }
    public string EmojiCode { get; set; }
    public string? Picture { get; set; }
    public List<EventType> EventTypes { get; set; }
}