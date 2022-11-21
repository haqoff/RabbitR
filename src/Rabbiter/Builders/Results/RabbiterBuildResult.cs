using System.ComponentModel.DataAnnotations;

namespace Rabbiter.Builders.Results;

/// <summary>
/// Represents the result of building instances.
/// </summary>
internal class RabbiterBuildResult
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="RabbiterBuildResult"/>.
    /// </summary>
    internal RabbiterBuildResult(IReadOnlyList<InstanceBuildResult> instances)
    {
        ThrowIfNotValid(instances);
        Instances = instances;
    }

    /// <summary>
    /// Configured instances.
    /// </summary>
    internal IReadOnlyList<InstanceBuildResult> Instances { get; }

    private static void ThrowIfNotValid(IReadOnlyList<InstanceBuildResult> instances)
    {
        var instanceNames = new HashSet<string>();

        foreach (var instance in instances)
        {
            if (!instanceNames.Add(instance.Name))
            {
                throw new ValidationException($"Instance names must be unique. Encountered \"{instance.Name}\" at least twice.");
            }
        }
    }
}