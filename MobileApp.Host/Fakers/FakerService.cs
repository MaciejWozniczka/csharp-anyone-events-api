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
        _usersIds =
        [
            "e6c2c10b-4667-4c1b-a5d6-596c0e7e1617",
            "3dea593b-d731-45eb-a62d-62d6673247ac",
            "a4b78472-32dc-4393-9bd9-953c8fc4cca9"
        ];
        _categories = [];
        _locationsIds =
        [
            new("1f35a742-4bf7-4271-a5ca-2fb781fb4c68"),
            new("0f6bd3b6-62e4-47b7-9b16-32076dd23a37"),
            new("6e0d1c50-dbb5-46b2-a385-c5dadf6434af"),
            new("6d71308a-0aa7-4033-a748-a808b210cfd3")
        ];
        _locations = [];
        _addressesIds =
        [
            new("e8f41bd0-85a1-4304-bbd0-f8001e193ed2"),
            new("27d4ec58-9291-4ea4-9b52-5ee85a2d3aab"),
            new("e1a2b4fc-6507-4add-bd7c-bd6a68091d7f"),
            new("cbc4bf65-2d81-4c2d-beac-b3637deaa935")
        ];
        _addresses = [];
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
            var age = 18 + _random.Next(20);

            var userEvent = new UserEvent
            {
                CategoryId = category.Id,
                EventTypeId = eventType.Id,
                CreatorId = _usersIds[_random.Next(_usersIds.Count)],
                EventDateTime = new DateTimeOffset(DateTime.UtcNow.Date.AddHours(i*i), TimeSpan.Zero),
                Duration = _random.Next(3) * 60,
                LocationId = _locations[_random.Next(_locations.Count)].Id,
                AddressId = _addresses[_random.Next(_addresses.Count)].Id,
                ShortDescription = $"{eventType.Name} już dzisiaj!",
                Description = $"Wbijaj pobawić się z nami na cotygodniowym wyjściu na {eventType.Name}. Zbiórka o wskazanej godzinie.",
                Picture = eventType.Picture ?? category.Picture,
                PeopleLimit = _random.Next(5 + 2),
                AgeFrom = age,
                AgeTo = _random.Next(20 + age),
                SexTypes = [(SexType)_random.Next(2)],
                IsActive = true,
                CreateDate = DateTimeOffset.UtcNow,
                IsDeleted = false
            };

            await _db.Events.AddAsync(userEvent, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}