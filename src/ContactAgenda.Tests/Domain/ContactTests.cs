using ContactAgenda.Domain;

namespace ContactAgenda.Tests.Domain;

public sealed class ContactTests
{
    [Fact]
    public void Create_TrimsAndTruncatesNameToFiftyTextElements()
    {
        var name = $"  {new string('a', 49)}👻more text  ";

        var contact = Contact.Create(name, "21999999999");

        Assert.Equal($"{new string('a', 49)}👻", contact.Name);
    }

    [Fact]
    public void Create_TruncatesFiveThousandCharacterName()
    {
        var contact = Contact.Create($"  {new string('a', 5000)}  ", "21999999999");

        Assert.Equal(new string('a', 50), contact.Name);
    }

    [Theory]
    [InlineData("(21) 99999-9999", "21999999999")]
    [InlineData("21 9999-9999", "2199999999")]
    [InlineData("0099999999", "0099999999")]
    [InlineData("99999999999", "99999999999")]
    public void Create_NormalizesPhoneSeparators(string input, string expected)
    {
        var contact = Contact.Create("Joana", input);

        Assert.Equal(expected, contact.Phone);
    }

    [Theory]
    [InlineData("219999999")]
    [InlineData("219999999999")]
    [InlineData("21-9999-ABCD")]
    public void Create_RejectsInvalidPhone(string phone)
    {
        var exception = Assert.Throws<ContactValidationException>(() => Contact.Create("Joana", phone));

        Assert.Contains(nameof(Contact.Phone), exception.Errors.Keys);
    }

    [Fact]
    public void Create_ReportsRequiredNameAndPhone()
    {
        var exception = Assert.Throws<ContactValidationException>(() => Contact.Create("  ", null));

        Assert.Contains(nameof(Contact.Name), exception.Errors.Keys);
        Assert.Contains(nameof(Contact.Phone), exception.Errors.Keys);
    }

    [Fact]
    public void NormalizeNameForSearch_IsCaseInsensitiveAndTrimsOuterWhitespace()
    {
        var upper = Contact.NormalizeNameForSearch("  Ágata Silva ");
        var lower = Contact.NormalizeNameForSearch("ágata silva");

        Assert.Equal(upper, lower);
    }

    [Fact]
    public void Update_ReplacesNameAndNormalizedPhone()
    {
        var contact = Contact.Create("Joana", "21999999999");
        var details = Contact.Normalize(" Maria ", "(11) 3456-7890");

        contact.Update(details);

        Assert.Equal("Maria", contact.Name);
        Assert.Equal("1134567890", contact.Phone);
        Assert.Equal("MARIA", contact.NormalizedName);
    }
}
