using Bunit;
using ContactAgenda.Application;
using ContactAgenda.Components.Pages;
using ContactAgenda.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactAgenda.Tests.Components;

public sealed class HomeTests
{
    [Fact]
    public async Task SaveSuccess_DisablesSaveUntilNewContactStarts()
    {
        using var context = new BunitContext();
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ContactDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var dbContext = new ContactDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        context.Services.AddSingleton(new ContactService(new EfContactRepository(dbContext)));

        var component = context.Render<Home>();

        Assert.Equal("Nome", component.Find("label[for='contact-name']").TextContent);
        Assert.Equal("Telefone completo", component.Find("label[for='contact-phone']").TextContent);
        component.Find("#contact-name").Change("Joana");
        component.Find("#contact-phone").Change("123");

        await component.InvokeAsync(() =>
            component.Find("form").TriggerEvent("onsubmit", new EventArgs()));

        component.WaitForAssertion(() =>
        {
            Assert.Contains("10 ou 11 dígitos", component.Find(".alert-danger").TextContent);
            Assert.False(component.Find("button[type='submit']").HasAttribute("disabled"));
        });

        component.Find("#contact-phone").Change("(21) 99999-9999");
        await component.InvokeAsync(() =>
            component.Find("form").TriggerEvent("onsubmit", new EventArgs()));

        component.WaitForAssertion(() =>
        {
            Assert.Equal("Contato salvo com sucesso!", component.Find(".alert-success").TextContent);
            Assert.True(component.Find("button[type='submit']").HasAttribute("disabled"));
        });

        component.Find("button[type='button']").Click();

        Assert.False(component.Find("button[type='submit']").HasAttribute("disabled"));
    }
}
