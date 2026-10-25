using ContactAgenda.Application;
using ContactAgenda.Domain;
using ContactAgenda.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ContactAgenda.Tests.Application;

public sealed class ContactServiceTests : IAsyncLifetime
{
    private SqliteConnection connection = null!;
    private ContactDbContext dbContext = null!;
    private ContactService service = null!;

    public async Task InitializeAsync()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ContactDbContext>()
            .UseSqlite(connection)
            .Options;
        dbContext = new ContactDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        service = new ContactService(new EfContactRepository(dbContext));
    }

    public async Task DisposeAsync()
    {
        await dbContext.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task CreateAsync_PersistsContactWithNormalizedPhone()
    {
        var contact = await service.CreateAsync("  Joana  ", "(21) 99999-9999");

        Assert.True(contact.Id > 0);
        Assert.Equal("Joana", contact.Name);
        Assert.Equal("21999999999", contact.Phone);
        Assert.Equal(contact.Id, (await dbContext.Contacts.SingleAsync()).Id);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicatePhoneAfterNormalization()
    {
        await service.CreateAsync("Joana", "(21) 99999-9999");

        await Assert.ThrowsAsync<DuplicatePhoneException>(
            () => service.CreateAsync("Maria", "21999999999"));
    }

    [Fact]
    public async Task Database_EnforcesUniqueNormalizedPhone()
    {
        dbContext.Contacts.Add(Contact.Create("Joana", "21999999999"));
        await dbContext.SaveChangesAsync();
        dbContext.Contacts.Add(Contact.Create("Maria", "(21) 99999-9999"));

        await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task UpdateAsync_AllowsKeepingOwnPhoneAndPreservesId()
    {
        var contact = await service.CreateAsync("Joana", "21999999999");

        var updated = await service.UpdateAsync(contact.Id, "Maria", "(21) 99999-9999");

        Assert.Equal(contact.Id, updated.Id);
        Assert.Equal("Maria", updated.Name);
    }

    [Fact]
    public async Task UpdateAsync_RejectsAnotherContactsPhone()
    {
        var first = await service.CreateAsync("Joana", "21999999999");
        var second = await service.CreateAsync("Maria", "1134567890");

        await Assert.ThrowsAsync<DuplicatePhoneException>(
            () => service.UpdateAsync(second.Id, "Maria", first.Phone));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsWhenContactDoesNotExist()
    {
        await Assert.ThrowsAsync<ContactNotFoundException>(
            () => service.UpdateAsync(404, "Maria", "21999999999"));
    }

    [Fact]
    public async Task SearchByNameAsync_ReturnsEveryCaseInsensitiveMatch()
    {
        await service.CreateAsync("Ágata Silva", "21999999999");
        await service.CreateAsync("ÁGATA SILVA", "1134567890");

        var contacts = await service.SearchByNameAsync("  áGata Silva ");

        Assert.Equal(2, contacts.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsWhenContactDoesNotExist()
    {
        await Assert.ThrowsAsync<ContactNotFoundException>(() => service.GetByIdAsync(404));
    }
}
