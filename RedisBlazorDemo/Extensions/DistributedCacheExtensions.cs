using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisBlazorDemo.Extensions
{
	public static class DistributedCacheExtensions
	{
		public static async Task SetRecordAsync<T>(this IDistributedCache cache, 
			string recordId, 
			T data, 
			TimeSpan? absoluteExpireTime = null, 
			TimeSpan? unusedExpireTime = null)
		{
			var options = new DistributedCacheEntryOptions();

			//固定到期時間
			//??代表:若是左邊的值是 null的話就使用右邊的值
			options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
			//若是在這個時間內該key完全沒使用就判定過期
			options.SlidingExpiration = unusedExpireTime;

			var jsonData = JsonSerializer.Serialize(data);
			await cache.SetStringAsync(recordId, jsonData, options);
		}

		public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
		{
			var jsonData = await cache.GetStringAsync(recordId);

			if(jsonData is null)
			{
				return default(T);
			}

			return JsonSerializer.Deserialize<T>(jsonData);
		}
	}
}
