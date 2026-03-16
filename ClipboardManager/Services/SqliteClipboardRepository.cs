using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClipboardManager.Data;
using ClipboardManager.Models;
using ClipboardManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClipboardManager.Services;

public class SqliteClipboardRepository : IClipboardRepository, IAsyncDisposable
{
    private readonly ClipboardDbContext _db;

    public SqliteClipboardRepository(string dbPath = "clipboard.db")
    {
        var dbFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "ClipboardManager"
);
        Directory.CreateDirectory(dbFolder); // ensures the folder exists
        dbPath = Path.Combine(dbFolder, "clipboard.db");

        _db = new ClipboardDbContext(dbPath);

        // Ensures the database and table exist without needing migrations.
        // Swap for _db.Database.MigrateAsync() if you add EF migrations later.
        _db.Database.EnsureCreated();
    }

    public async Task SaveEntryAsync(string content)
    {
        _db.ClipboardEntries.Add(new ClipboardEntry(content));
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<string>> LoadAllAsync()
    {
        return await _db.ClipboardEntries
            .OrderByDescending(e => e.CopiedAt)
            .Select(e => e.Text)
            .ToListAsync();
    }

    public async Task DeleteEntryAsync(string content)
    {
        var entry = await _db.ClipboardEntries
            .FirstOrDefaultAsync(e => e.Text == content);

        if (entry is not null)
        {
            _db.ClipboardEntries.Remove(entry);
            await _db.SaveChangesAsync();
        }
    }

    public async Task ClearAllAsync()
    {
        await _db.ClipboardEntries.ExecuteDeleteAsync();
    }

    public async ValueTask DisposeAsync() => await _db.DisposeAsync();
}