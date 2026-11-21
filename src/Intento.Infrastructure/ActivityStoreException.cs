namespace Intento.Infrastructure;

/// <summary>
/// A recoverable persistence failure. The original exception is available as InnerException.
/// </summary>
public sealed class ActivityStoreException : Exception
{
    public ActivityStoreException(string message, string filePath, Exception innerException)
        : base(message, innerException)
    {
        FilePath = filePath;
    }

    public string FilePath { get; }
}
