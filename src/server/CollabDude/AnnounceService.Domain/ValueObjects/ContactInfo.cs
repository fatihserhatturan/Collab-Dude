namespace AnnounceService.Domain.ValueObjects;

public class ContactInfo : ValueObject
{
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Website { get; private set; }
    public string? SocialMedia { get; private set; }

    private ContactInfo() { } // EF Core için

    public ContactInfo(string email, string? phone = null, string? website = null, string? socialMedia = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        Email = email;
        Phone = phone;
        Website = website;
        SocialMedia = socialMedia;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
        yield return Phone ?? string.Empty;
        yield return Website ?? string.Empty;
        yield return SocialMedia ?? string.Empty;
    }
}