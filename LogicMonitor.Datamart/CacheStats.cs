
namespace LogicMonitor.Datamart;

public class CacheStats(string name)
{
	public int Misses { get; private set; }

	public int Hits { get; private set; }

	public void AddMiss() => Misses++;

	public void AddHit() => Hits++;

	public void Reset()
	{
		Misses = 0;
		Hits = 0;
	}

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