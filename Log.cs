namespace AergiaConfigurator;

/// <summary>
/// Provides a static logging mechanism that dispatches log messages to the appropriate handlers
/// for error, warning, and informational messages.
/// </summary>
public static class Log
{
	public delegate void Writer(Location location, string msg);
	private static void EmptyWeiter(Location location, string msg) { }
	public static Writer Error = EmptyWeiter;
	public static Writer Warn = EmptyWeiter;
	public static Writer Info = EmptyWeiter;
}
