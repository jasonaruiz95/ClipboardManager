using ClipboardManager.Models;
using Microsoft.EntityFrameworkCore;


namespace ClipboardManager.Data;

public class ClipboardDbContext : DbContext
{
    public DbSet<ClipboardEntry> ClipboardEntries => Set<ClipboardEntry>();

    private readonly string _dbPath;

    public ClipboardDbContext(string dbPath = "clipboard.db")
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClipboardEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired();
            entity.Property(e => e.CopiedAt).IsRequired();
        });
    }
}