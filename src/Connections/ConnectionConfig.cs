using System.ComponentModel.DataAnnotations;

namespace RabbitR.Connections;

/// <summary>
/// Represents the connection config to the Rabbit MQ server.
/// </summary>
public class ConnectionConfig : IValidatableObject
{
    /// <summary>
    /// Host name.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "The host name must be specified.")]
    public string HostName { get; set; } = null!;   
    
    /// <summary>
    /// Host name.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "The virtual host must be specified.")]
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Port.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The port must be correct.")]
    public int Port { get; set; }

    /// <summary>
    /// User name.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "The userName must be specified.")]
    public string UserName { get; set; } = null!;

    /// <summary>
    /// Password.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "The password must be specified.")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// The delay before trying to connect again if connection fails.
    /// </summary>
    [Required]
    public TimeSpan RetryTimeout { get; set; }

    /// <summary>
    /// The maximum number of reconnects after which an error will be thrown.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Maximum reconnect count must be equal or greater than 0.")]
    public int MaxReconnectCount { get; set; }

    /// <summary>
    /// Performs additional validation.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RetryTimeout <= TimeSpan.Zero)
        {
            yield return new ValidationResult("Retry timeout must be greater than 0.");
        }
    }

    /// <summary>
    /// Gets string representation.
    /// </summary>
    public override string ToString()
    {
        return $"{HostName}:{Port}";
    }
}