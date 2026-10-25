using ContactAgenda.Domain;

namespace ContactAgenda.Application;

public sealed class ContactService(IContactRepository contacts)
{
    public async Task<Contact> CreateAsync(string? name, string? phone, CancellationToken cancellationToken = default)
    {
        var contact = Contact.Create(name, phone);

        if (await contacts.PhoneExistsAsync(contact.Phone, cancellationToken: cancellationToken))
        {
            throw new DuplicatePhoneException();
        }

        await contacts.AddAsync(contact, cancellationToken);
        return contact;
    }

    public async Task<Contact> UpdateAsync(
        int id,
        string? name,
        string? phone,
        CancellationToken cancellationToken = default)
    {
        var contact = await contacts.FindByIdAsync(id, cancellationToken)
            ?? throw new ContactNotFoundException(id);
        var details = Contact.Normalize(name, phone);

        if (await contacts.PhoneExistsAsync(details.Phone, id, cancellationToken))
        {
            throw new DuplicatePhoneException();
        }

        contact.Update(details);
        await contacts.SaveChangesAsync(cancellationToken);
        return contact;
    }

    public async Task<IReadOnlyList<Contact>> SearchByNameAsync(
        string? name,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = Contact.NormalizeNameForSearch(name);
        return await contacts.SearchByNameAsync(normalizedName, cancellationToken);
    }

    public async Task<Contact> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await contacts.FindByIdAsync(id, cancellationToken)
            ?? throw new ContactNotFoundException(id);
    }
}
