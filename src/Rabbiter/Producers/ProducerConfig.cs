using System.ComponentModel.DataAnnotations;

namespace Rabbiter.Producers;

/// <summary>
/// Represents a producer config.
/// </summary>
public class ProducerConfig
{
    /// <summary>
    /// The maximum number of producers that can be created in the pool for parallel publishing.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "The maximum number of producers must be at least 1.")]
    public int MaxPoolLength { get; set; }
}