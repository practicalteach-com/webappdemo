using Microsoft.EntityFrameworkCore;

public class ContactDbContext(DbContextOptions<ContactDbContext> options) : DbContext(options)
{
    public DbSet<WebApp.Model.Contact> Contact { get; set; } = default!;
}
