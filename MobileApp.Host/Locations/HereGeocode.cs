namespace MobileApp.Host.Locations;

public class HereGeocode
{
    public List<Item> Items { get; set; }
}

public class Access
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class FieldScore
{
    public int City { get; set; }
    public List<int> Streets { get; set; }
    public int HouseNumber { get; set; }
}

public class Item
{
    public string Title { get; set; }
    public string Id { get; set; }
    public string ResultType { get; set; }
    public string HouseNumberType { get; set; }
    public Address Address { get; set; }
    public Position Position { get; set; }
    public List<Access> Access { get; set; }
    public MapView MapView { get; set; }
    public Scoring Scoring { get; set; }
}

public class MapView
{
    public double West { get; set; }
    public double South { get; set; }
    public double East { get; set; }
    public double North { get; set; }
}

public class Position
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class Scoring
{
    public int QueryScore { get; set; }
    public FieldScore FieldScore { get; set; }
}