﻿using Rabbiter.Builders.Results;
using Rabbiter.Connections;
using Rabbiter.Utils;
using RabbitMQ.Client;

namespace Rabbiter.Producers;

/// <summary>
/// Represents the storage mechanism for created channels for an instance producer.
/// </summary>
internal class ProducerModelPoolStorage : IProducerModelPoolStorage
{
    private readonly Dictionary<string, LimitedPool<IModel>> _modelDictionary;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ProducerModelPoolStorage"/>.
    /// </summary>
    internal ProducerModelPoolStorage(IConnectionHolder connectionHolder, IReadOnlyList<InstanceBuildResult> instances)
    {
        _modelDictionary = new Dictionary<string, LimitedPool<IModel>>();
        FillPools(connectionHolder, instances);
    }

    /// <summary>
    /// Gets the channel pool for the specified instance named <paramref name="instanceName"/>.
    /// </summary>
    public LimitedPool<IModel> Get(string instanceName)
    {
        if (!_modelDictionary.TryGetValue(instanceName, out var pool))
        {
            throw new InvalidOperationException($"Instance named \"{instanceName}\" did not have a registered producer.");
        }

        return pool;
    }

    /// <summary>
    /// Dispose all models.
    /// </summary>
    public void Dispose()
    {
        foreach (var pool in _modelDictionary.Values)
        {
            foreach (var model in pool)
            {
                model.Dispose();
            }
        }
    }

    private void FillPools(IConnectionHolder connectionHolder, IReadOnlyList<InstanceBuildResult> instances)
    {
        foreach (var instance in instances)
        {
            if (instance.Producer is null)
            {
                continue;
            }

            var pool = new LimitedPool<IModel>(instance.Producer.Config.MaxPoolLength, () =>
            {
                var connection = connectionHolder.GetOrCreateConnection(instance);
                return connection.CreateModel();
            });

            _modelDictionary.Add(instance.Name, pool);
        }
    }
}