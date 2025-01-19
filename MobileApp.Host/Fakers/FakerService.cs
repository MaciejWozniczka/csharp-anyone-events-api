namespace MobileApp.Host.Fakers;

public interface IFakerService
{
    Task CreateFakeEvents(CancellationToken cancellationToken);
}
public class FakerService : IFakerService
{
    private readonly DataContext _db;
    private readonly Random _random;
    private readonly List<string> _usersIds;
    private List<Category> _categories;
    private readonly List<Guid> _locationsIds;
    private List<Location> _locations;
    private readonly List<Guid> _addressesIds;
    private List<Address> _addresses;
    public FakerService(DataContext db)
    {
        _db = db;
        _random = new Random();
        _usersIds = new List<string>
        {
            "a0e79c3d-7949-4f50-b099-f23505ddd826",
            "4f2235e2-2f52-4618-bf17-014dd26529a8",
            "0029ae26-f73f-49d9-b7bf-2b410ff4493b"
        };
        _categories = new List<Category>();
        _locationsIds = new List<Guid>
        {
            new("536c96c6-8fed-4bca-aa13-a3c8a49a8f99"),
            new("666cf786-cee2-496b-ae19-cc75db809783"),
            new("0e29b772-33b1-4de0-8d29-e6ed0e820512"),
            new("949572e1-02b4-4aec-9530-ab7a1afb4c0e"),
            new("3aa5e8ca-f84b-49a3-b03b-cac6c70857a8")
        };
        _locations = new List<Location>();
        _addressesIds = new List<Guid>
        {
            new("78ddbf57-bfdd-48d0-8b7d-9e7f10e376c8"),
            new("efcbcd7f-99fc-40d5-8906-967ce8b165ed"),
            new("0eea46a7-93b2-40ba-bf77-c69a419c8834"),
            new("e11743dc-5ff5-44c8-9725-a6c85f11f994"),
            new("03127443-b32b-4e83-8cdb-a70d0f1e7c18")
        };
        _addresses = new List<Address>();
    }

    public async Task CreateFakeEvents(CancellationToken cancellationToken)
    {
        _categories = await _db.Categories
            .Where(c => !c.IsDeleted)
            .Include(c => c.EventTypes)
            .ToListAsync(cancellationToken);

        _locations = await _db.Locations
            .Where(l => _locationsIds.Contains(l.Id))
            .ToListAsync(cancellationToken);

        _addresses = await _db.Addresses
            .Where(a => _addressesIds.Contains(a.Id))
            .ToListAsync(cancellationToken);

        for (var i = 0; i < 5; i++)
        {
            var category = _categories[_random.Next(_categories.Count)];
            var eventType = category.EventTypes[_random.Next(category.EventTypes.Count)];
            var age = _random.Next(20 + 18);

            var userEvent = new UserEvent
            {
                CategoryId = category.Id,
                EventTypeId = eventType.Id,
                CreatorId = _usersIds[_random.Next(_usersIds.Count)],
                EventDateTime = new DateTimeOffset(DateTime.UtcNow.Date.AddHours(i*i)),
                Duration = _random.Next(3) * 60,
                Location = _locations[_random.Next(_locations.Count)],
                Address = _addresses[_random.Next(_addresses.Count)],
                ShortDescription = $"{eventType.Name} już dzisiaj!",
                Description = $"Wbijaj pobawić się z nami na cotygodniowym wyjściu na {eventType.Name}. Zbiórka o wskazanej godzinie.",
                Picture = eventType.Picture ?? category.Picture,
                PeopleLimit = _random.Next(5 + 2),
                AgeFrom = age,
                AgeTo = _random.Next(20 + age),
                SexTypes = new List<SexType> { (SexType)_random.Next(2) },
                IsActive = true,
                CreateDate = DateTimeOffset.Now,
                IsDeleted = false
            };

            await _db.Events.AddAsync(userEvent, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}