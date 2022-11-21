using Rabbiter.Producers;
using Rabbiter.Utils;

namespace Rabbiter.Builders.Results;

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