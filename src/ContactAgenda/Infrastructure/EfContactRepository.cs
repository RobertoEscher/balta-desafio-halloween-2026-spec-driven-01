using ContactAgenda.Application;
using ContactAgenda.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ContactAgenda.Infrastructure;

public sealed class EfContactRepository(ContactDbContext dbContext) : IContactRepository
{
    public Task<bool> PhoneExistsAsync(
        string phone,
        int? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Contacts.Where(contact => contact.Phone == phone);
        if (excludingId.HasValue)
        {
            query = query.Where(contact => contact.Id != excludingId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<Contact?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Contacts.SingleOrDefaultAsync(contact => contact.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Contact>> SearchByNameAsync(
        string normalizedName,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Contacts
            .Where(contact => contact.NormalizedName == normalizedName)
            .OrderBy(contact => contact.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        dbContext.Contacts.Add(contact);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraint(exception))
        {
            throw new DuplicatePhoneException();
        }
    }

    private static bool IsUniqueConstraint(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException
        {
            SqliteErrorCode: 19,
            SqliteExtendedErrorCode: 2067 or 1555
        };
    }
}
