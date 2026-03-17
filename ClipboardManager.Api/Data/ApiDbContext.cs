using ClipboardManager.Api.Models;
using ClipboardManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClipboardManager.Api.Data;

public class ApiDbContext : IdentityDbContext<ApplicationUser>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
    }

    public DbSet<ClipboardEntry> ClipboardEntries => Set<ClipboardEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ClipboardEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired();
            entity.Property(e => e.CopiedAt).IsRequired();
            
            // Add a UserId to associate entries with users if needed, 
            // but the requirement said "replica", so let's keep it simple first.
            // Actually, for a multi-user API, we definitely want to separate data.
            entity.Property<string>("UserId");
        });
    }
}
