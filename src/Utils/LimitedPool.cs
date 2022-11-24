using System.Collections;
using System.Collections.Concurrent;

namespace RabbitR.Utils;

/// <summary>
/// Provides a limited object pool in which objects can be created as needed.
/// </summary>
/// <typeparam name="T">Type.</typeparam>
internal class LimitedPool<T> : IEnumerable<T>
{
    private readonly Func<T> _factory;
    private readonly ConcurrentBag<T> _freeItems;
    private readonly ConcurrentBag<T> _allItems;
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Initializes a new instance of the class <see cref="LimitedPool{T}"/>.
    /// </summary>
    /// <param name="maxSize">The maximum number of objects that can be created at all times.</param>
    /// <param name="factory">A function that returns a new instance of an object.</param>
    /// <exception cref="ArgumentOutOfRangeException">An error that occurs if the specified maximum size of objects in the pool is less than 1.</exception>
    internal LimitedPool(int maxSize, Func<T> factory)
    {
        if (maxSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSize), "Max pool size cannot be less than 1.");
        }

        _factory = factory;
        _freeItems = new ConcurrentBag<T>();
        _semaphore = new SemaphoreSlim(maxSize, maxSize);
        _allItems = new ConcurrentBag<T>();
    }

    /// <summary>
    /// Gets an existing object or creates a new one if all others are busy and more can be created.
    /// If no more can be created, waits until a free object is available.
    /// </summary>
    internal async Task<T> TakeAsync()
    {
        await _semaphore.WaitAsync();

        // first we try to take the existing one.
        if (_freeItems.TryTake(out var beforeCreateItem))
        {
            return beforeCreateItem;
        }

        // create new one
        var newItem = _factory();
        _allItems.Add(newItem);
        return newItem;
    }

    /// <summary>
    /// Returns an object back to the pool.
    /// So other can use it again.
    /// Note that there is no tracking that this object was created within this pool.
    /// </summary>
    internal void Return(T item)
    {
        _freeItems.Add(item);
        _semaphore.Release();
    }

    /// <summary>
    /// Enumerates all created objects within the pool. 
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        return _allItems.GetEnumerator();
    }

    /// <summary>
    /// Enumerates all created objects within the pool. 
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}