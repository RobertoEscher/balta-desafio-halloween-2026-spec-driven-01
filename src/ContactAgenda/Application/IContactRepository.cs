using ContactAgenda.Domain;

namespace ContactAgenda.Application;

public interface IContactRepository
{
    Task<bool> PhoneExistsAsync(string phone, int? excludingId = null, CancellationToken cancellationToken = default);
    Task<Contact?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Contact>> SearchByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
