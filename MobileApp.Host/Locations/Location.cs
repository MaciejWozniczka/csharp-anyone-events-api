using System.ComponentModel.DataAnnotations.Schema;

namespace MobileApp.Host.Locations;

public class Location : BaseModel
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Distance { get; set; }
    public string UserId { get; set; }
}