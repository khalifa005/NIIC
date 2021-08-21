using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Application
{
    public abstract class CacheObject
    {
        public const string Namespace = "NIIC.Core";

        protected IDistributedCache _cache;

        public virtual string Key(string val = "") => "";

        protected CacheObject(IDistributedCache cache)
        {
            _cache = cache;
        }

        private static string FormatKey(string k)
        {
            return Namespace + "." + k;
        }

        protected async Task<T> GetAsync<T>((string key, DistributedCacheEntryOptions options) info, Func<Task<T>> valueGetterAsync) where T : class
        {
            var key = FormatKey(info.key);

            var v = await _cache.GetAsync(key);
            if (v is null)
            {
                var freshValue = await valueGetterAsync();
                using (var m = new MemoryStream())
                {
                    await Json.SerializeAsync(m, freshValue);
                    await _cache.SetAsync(key, m.ToArray(), info.options);

                    return freshValue;
                }
            }
            else
            {
                return Json.Deserialize<T>(v);
            }
        }

        protected DistributedCacheEntryOptions AbsoluteExpiration(TimeSpan expiration)
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
        }

        protected DistributedCacheEntryOptions AbsoluteOneHour => AbsoluteExpiration(TimeSpan.FromHours(1));

        protected DistributedCacheEntryOptions AbsoluteTwoHours => AbsoluteExpiration(TimeSpan.FromHours(2));

        protected DistributedCacheEntryOptions AbsoluteOneDay => AbsoluteExpiration(TimeSpan.FromDays(1));

        protected DistributedCacheEntryOptions AbsoluteHundredDays => AbsoluteExpiration(TimeSpan.FromDays(100));

        protected DistributedCacheEntryOptions AbsoluteThousandDays => AbsoluteExpiration(TimeSpan.FromDays(1000));

        protected DistributedCacheEntryOptions SlidingExpiration(TimeSpan expiration)
        {
            return new DistributedCacheEntryOptions
            {
                SlidingExpiration = expiration
            };
        }

        protected DistributedCacheEntryOptions Sliding30Minutes => SlidingExpiration(TimeSpan.FromMinutes(30));

        public Task RemoveAsync(string key)
        {
            return _cache.RemoveAsync(FormatKey(key));
        }
    }
}
