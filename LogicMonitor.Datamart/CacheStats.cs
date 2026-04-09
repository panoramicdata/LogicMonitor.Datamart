
namespace LogicMonitor.Datamart;

/// <summary>
/// Tracks cache hit/miss statistics for a named cache.
/// </summary>
/// <param name="name">The name of the cache being tracked.</param>
public class CacheStats(string name)
{
	/// <summary>
	/// Gets the number of cache misses.
	/// </summary>
	public int Misses { get; private set; }

	/// <summary>
	/// Gets the number of cache hits.
	/// </summary>
	public int Hits { get; private set; }

	/// <summary>
	/// Records a cache miss.
	/// </summary>
	public void AddMiss() => Misses++;

	/// <summary>
	/// Records a cache hit.
	/// </summary>
	public void AddHit() => Hits++;

	/// <summary>
	/// Resets both hit and miss counters to zero.
	/// </summary>
	public void Reset()
	{
		Misses = 0;
		Hits = 0;
	}

	/// <summary>
	/// Adds the hit and miss counts from another <see cref="CacheStats"/> instance.
	/// </summary>
	/// <param name="cacheStats">The cache stats to merge in.</param>
	public void Add(CacheStats cacheStats)
	{
		Misses += cacheStats.Misses;
		Hits += cacheStats.Hits;
	}

	/// <summary>
	/// Log the hit/miss ratio
	/// </summary>
	/// <param name="logger"></param>
	public void Log(ILogger logger)
	{
		if (Hits + Misses > 0)
		{
			logger.LogInformation(
				"Cache hit stats for {Name}: {CacheHits} hits, {CacheMisses} misses ({CacheHitPercentage:F2}%)",
				name,
				Hits,
				Misses,
				(double)Hits / (Hits + Misses) * 100
			);
		}
		else
		{
			logger.LogInformation("No cache hits or misses");
		}
	}
}