using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ContactAgenda.Endpoints;
using ContactAgenda.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContactAgenda.Tests.Endpoints;

public sealed class ContactEndpointsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task PostCadastro_CreatesContact()
    {
        using var factory = new ContactApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/cadastro", new ContactRequest("Joana", "(21) 99999-9999"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var contact = await response.Content.ReadFromJsonAsync<ContactResponse>(JsonOptions);
        Assert.NotNull(contact);
        Assert.True(contact.Id > 0);
        Assert.Equal("21999999999", contact.Phone);
        Assert.DoesNotContain("normalizedName", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetBuscaByName_ReturnsAllCaseInsensitiveMatches()
    {
        using var factory = new ContactApiFactory();
        using var client = factory.CreateClient();
        await CreateContact(client, "Ágata Silva", "21999999999");
        await CreateContact(client, "ÁGATA SILVA", "1134567890");

        var contacts = await client.GetFromJsonAsync<ContactResponse[]>("/api/busca?nome=%C3%A1gata%20silva", JsonOptions);

        Assert.NotNull(contacts);
        Assert.Equal(2, contacts.Length);
    }

    [Fact]
    public async Task GetBuscaById_ReturnsContact()
    {
        using var factory = new ContactApiFactory();
        using var client = factory.CreateClient();
        var created = await CreateContact(client, "Joana", "21999999999");

        var contact = await client.GetFromJsonAsync<ContactResponse>($"/api/busca/{created.Id}", JsonOptions);

        Assert.NotNull(contact);
        Assert.Equal(created.Id, contact.Id);
        Assert.Equal(created.Name, contact.Name);
    }

    [Fact]
    public async Task PutCadastro_UpdatesContactWithoutChangingId()
    {
        using var factory = new ContactApiFactory();
        using var client = factory.CreateClient();
        var created = await CreateContact(client, "Joana", "21999999999");

        var response = await client.PutAsJsonAsync(
            $"/api/cadastro/{created.Id}",
            new ContactRequest("Maria", "1134567890"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<ContactResponse>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Maria", updated.Name);
    }

    [Fact]
    public async Task PostCadastro_ValidationFailureReturnsProblemDetails400()
    {
        using var factory = new ContactApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/cadastro", new ContactRequest("", "123"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task PostCadastro_DuplicateNormalizedPhoneReturnsProblemDetails400()
    {
        using var factory = new ContactApiFactory();
        using var client = factory.CreateClient();
        await CreateContact(client, "Joana", "(21) 99999-9999");

        var response = await client.PostAsJsonAsync(
            "/api/cadastro",
            new ContactRequest("Maria", "21999999999"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    private static async Task<ContactResponse> CreateContact(HttpClient client, string name, string phone)
    {
        var response = await client.PostAsJsonAsync("/api/cadastro", new ContactRequest(name, phone));
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ContactResponse>(JsonOptions))!;
    }

    private sealed class ContactApiFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection connection = new("Data Source=:memory:");

        public ContactApiFactory()
        {
            connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<ContactDbContext>));
                services.RemoveAll<DbContextOptions<ContactDbContext>>();
                services.RemoveAll<ContactDbContext>();
                services.AddDbContext<ContactDbContext>(options => options.UseSqlite(connection));
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                connection.Dispose();
            }
        }
    }
}
