namespace AnnounceService.Domain.ValueObjects;

public class Location : ValueObject
{
    public string City { get; private set; }
    public string Country { get; private set; }
    public bool IsRemote { get; private set; }

    private Location() { } // EF Core için

    public Location(string city, string country, bool isRemote = false)
    {
        City = city ?? throw new ArgumentNullException(nameof(city));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        IsRemote = isRemote;
    }

    public static Location Remote()
    {
        return new Location("Remote", "Global", true);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return Country;
        yield return IsRemote;
    }

    public override string ToString()
    {
        return IsRemote ? "Remote" : $"{City}, {Country}";
    }
}