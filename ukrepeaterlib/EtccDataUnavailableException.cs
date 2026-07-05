namespace ukrepeaterlib;

/// <summary>
/// Thrown when the upstream ETCC repeater API is unreachable and no cached data is
/// available to fall back on. Callers should surface this to clients as an HTTP 503
/// rather than a 500, since it is a transient upstream condition.
/// </summary>
public sealed class EtccDataUnavailableException(string message, Exception? innerException = null)
    : Exception(message, innerException);
