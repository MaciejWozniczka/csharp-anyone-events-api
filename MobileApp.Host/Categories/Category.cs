using MobileApp.Host.Events;

namespace MobileApp.Host.Categories;

public class Category : BaseModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Picture { get; set; }
    public List<EventType> EventTypes { get; set; }
    public List<Event> Events { get; set; }
}