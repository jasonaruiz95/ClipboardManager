namespace ClipboardManager.Services;

public enum DatabaseBackend { Sqlite, Remote }

public class DatabaseConfig
{
    public DatabaseBackend Backend { get; init; } = DatabaseBackend.Sqlite;
    public string SqlitePath { get; init; } = "clipboard.db";
    public string RemoteConnectionString { get; init; } = string.Empty;
}