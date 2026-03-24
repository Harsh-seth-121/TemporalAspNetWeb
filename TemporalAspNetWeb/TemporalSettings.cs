namespace TemporalAspNetWeb;

/// <summary>
/// Temporal settings for configuring the Temporal client and worker.
/// </summary>
public class TemporalSettings
{
    /// <summary>
    /// Gets or sets the target host for the Temporal service. If not provided, defaults to "localhost:7233".
    /// </summary>
    public string? ClientTargetHost { get; set; } = null;

    /// <summary>
    /// Gets or sets the Temporal namespace to use. If not provided, defaults to "default".
    /// </summary>
    public string? ClientNamespace { get; set; } = null;

    /// <summary>
    /// Gets or sets the API key for authenticating with the Temporal service. If not provided, the client will attempt to connect without authentication.
    /// </summary>
    public string? ApiKey { get; set; } = null;
}