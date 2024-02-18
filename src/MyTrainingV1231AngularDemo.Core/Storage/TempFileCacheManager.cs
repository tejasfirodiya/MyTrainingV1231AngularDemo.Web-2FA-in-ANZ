using System;
using Abp.Runtime.Caching;

namespace MyTrainingV1231AngularDemo.Storage
{
    public class TempFileCacheManager : ITempFileCacheManager
    {
        private const string TempFileCacheName = "TempFileCacheName";

        private readonly ITypedCache<string, TempFileInfo> _cache;

        public TempFileCacheManager(ICacheManager cacheManager)
        {
            _cache = cacheManager.GetCache<string, TempFileInfo>(TempFileCacheName);
        }

        public void SetFile(string token, byte[] content)
        {
            _cache.Set(token, new TempFileInfo(content), TimeSpan.FromMinutes(1)); // expire time is 1 min by default
        }

        public byte[] GetFile(string token)
        {
            var cache = _cache.GetOrDefault(token);
            return cache?.File;
        }

        public void SetFile(string token, TempFileInfo info)
        {
            _cache.Set(token, info, TimeSpan.FromMinutes(1)); // expire time is 1 min by default
        }

        public TempFileInfo GetFileInfo(string token)
        {
            return _cache.GetOrDefault(token);
        }
    }
}