namespace MobileApp.Host.Categories;

public class Category : BaseModel
{
    public Category()
    {
        EventTypes = new List<EventType>();
    }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public List<EventType> EventTypes { get; set; }
}