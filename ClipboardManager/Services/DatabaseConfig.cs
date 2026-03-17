namespace ClipboardManager.Services;

public enum DatabaseBackend { Sqlite, Remote, ApiSync }

public class DatabaseConfig
{
    public DatabaseBackend Backend { get; init; } = DatabaseBackend.Sqlite;
    public string SqlitePath { get; init; } = "clipboard.db";
    public string RemoteConnectionString { get; init; } = string.Empty;
    public string ApiBaseUrl { get; init; } = "https://localhost:5001";
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}