using System.Security.Claims;
using ClipboardManager.Api.Data;
using ClipboardManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClipboardManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClipboardController : ControllerBase
{
    private readonly ApiDbContext _context;

    public ClipboardController(ApiDbContext context)
    {
        _context = context;
    }

    private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClipboardEntry>>> GetEntries()
    {
        return await _context.ClipboardEntries
            .Where(e => EF.Property<string>(e, "UserId") == UserId)
            .OrderByDescending(e => e.CopiedAt)
            .ToListAsync();
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncEntries(List<ClipboardEntry> entries)
    {
        var existingEntries = await _context.ClipboardEntries
            .Where(e => EF.Property<string>(e, "UserId") == UserId)
            .ToListAsync();

        foreach (var entry in entries)
        {
            // Simple sync logic: if text doesn't exist, add it.
            // In a real app, we'd use better unique identifiers.
            if (!existingEntries.Any(e => e.Text == entry.Text))
            {
                var newEntry = new ClipboardEntry(entry.Text)
                {
                    Time = entry.Time,
                    CopiedAt = entry.CopiedAt
                };
                _context.Entry(newEntry).Property("UserId").CurrentValue = UserId;
                _context.ClipboardEntries.Add(newEntry);
            }
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<ClipboardEntry>> PostEntry(ClipboardEntry entry)
    {
        var newEntry = new ClipboardEntry(entry.Text)
        {
            Time = entry.Time,
            CopiedAt = entry.CopiedAt
        };
        _context.Entry(newEntry).Property("UserId").CurrentValue = UserId;
        _context.ClipboardEntries.Add(newEntry);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEntries), new { id = newEntry.Id }, newEntry);
    }
}
