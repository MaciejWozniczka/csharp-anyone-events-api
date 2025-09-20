using AnyOneApi.Host.EventTypes;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.Categories;

public class Category : BaseModel
{
    public string Name { get; set; }
    public string EmojiCode { get; set; }
    public string? Picture { get; set; }
    public List<EventType> EventTypes { get; set; } = [];
}