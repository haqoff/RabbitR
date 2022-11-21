using System.ComponentModel.DataAnnotations;

namespace Rabbiter.Producers;

/// <summary>
/// Represents a producer config.
/// </summary>
public class ProducerConfig : IValidatableObject
{
    /// <summary>
    /// The maximum number of producers that can be created in the pool for parallel publishing.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "The maximum number of producers must be at least 1.")]
    public int MaxPoolLength { get; set; }

    /// <summary>
    /// The maximum number of attempts to publishing.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int MaxRetryCount { get; set; }

    /// <summary>
    /// Delay between publishing attempts.
    /// </summary>
    public TimeSpan AttemptDelay { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AttemptDelay <= TimeSpan.Zero)
        {
            yield return new ValidationResult("The delay between publishing attempts must be greater than 0.");
        }
    }
}