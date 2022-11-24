using RabbitR.Producers;
using RabbitR.Utils;

namespace RabbitR.Builders.Instances.Producers;

/// <summary>
/// Represents the result of building producer.
/// </summary>
internal class ProducerBuildResult
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="RabbiterBuildResult"/>.
    /// </summary>
    internal ProducerBuildResult(ProducerConfig config)
    {
        RabbiterValidator.ThrowIfNotValid(config);
        Config = config;
    }

    /// <summary>
    /// Producer config.
    /// </summary>
    internal ProducerConfig Config { get; }
}