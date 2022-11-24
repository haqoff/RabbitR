using RabbitR.Builders.Instances;
using RabbitR.Connections;

namespace RabbitR.Builders;

/// <summary>
/// It is a connection builder that allows you to add multiple connection instances.
/// </summary>
public class RabbiterBuilder
{
    private readonly List<InstanceBuildResult> _instances = new();

    /// <summary>
    /// Initializes a new instance of the class <see cref="RabbiterBuilder"/>.
    /// </summary>
    internal RabbiterBuilder()
    {
    }

    /// <summary>
    /// Adds a new connection instance.
    /// </summary>
    /// <param name="name">Instance name. Must be unique among others.</param>
    /// <param name="bindConfig">Action that configures connection config.</param>
    /// <param name="builderAction">Action that configures instance.</param>
    public RabbiterBuilder AddInstance(string name, Action<ConnectionConfig> bindConfig, Action<InstanceBuilder> builderAction)
    {
        var config = new ConnectionConfig();
        bindConfig(config);

        var instanceBuilder = new InstanceBuilder(name, config);
        builderAction(instanceBuilder);

        _instances.Add(instanceBuilder.Build());
        return this;
    }

    /// <summary>
    /// Adds a new connection instance with default instance name.
    /// </summary>
    /// <param name="bindConfig">Action that configures connection config.</param>
    /// <param name="builderAction">Action that configures instance.</param>
    public RabbiterBuilder AddInstance(Action<ConnectionConfig> bindConfig, Action<InstanceBuilder> builderAction)
    {
        return AddInstance(InstanceConstants.DefaultName, bindConfig, builderAction);
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal RabbiterBuildResult Build()
    {
        return new RabbiterBuildResult(_instances);
    }
}