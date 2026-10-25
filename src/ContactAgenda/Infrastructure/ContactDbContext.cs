using ContactAgenda.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContactAgenda.Infrastructure;

public sealed class ContactDbContext(DbContextOptions<ContactDbContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var contact = modelBuilder.Entity<Contact>();
        contact.HasKey(item => item.Id);
        contact.Property(item => item.Id).ValueGeneratedOnAdd();
        contact.Property(item => item.Name).HasMaxLength(50).IsRequired();
        contact.Property(item => item.Phone).HasMaxLength(11).IsRequired();
        contact.Property(item => item.NormalizedName).HasMaxLength(50).IsRequired();
        contact.HasIndex(item => item.Phone).IsUnique();
        contact.HasIndex(item => item.NormalizedName);
    }
}
