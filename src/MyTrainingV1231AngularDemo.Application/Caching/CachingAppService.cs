using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using MyTrainingV1231AngularDemo.Authorization;
using MyTrainingV1231AngularDemo.Caching.Dto;

namespace MyTrainingV1231AngularDemo.Caching
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Host_Maintenance)]
    public class CachingAppService : MyTrainingV1231AngularDemoAppServiceBase, ICachingAppService
    {
        private readonly ICacheManager _cacheManager;

        public CachingAppService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public ListResultDto<CacheDto> GetAllCaches()
        {
            var caches = _cacheManager.GetAllCaches()
                                        .Select(cache => new CacheDto
                                        {
                                            Name = cache.Name
                                        })
                                        .ToList();

            return new ListResultDto<CacheDto>(caches);
        }

        public async Task ClearCache(EntityDto<string> input)
        {
            var cache = _cacheManager.GetCache(input.Id);
            await cache.ClearAsync();
        }

        public async Task ClearAllCaches()
        {
            if (!CanClearAllCaches())
            {
                throw new ApplicationException("This method can be used only with Memory Cache!");
            }
            
            var caches = _cacheManager.GetAllCaches();
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }
        }
        
        public bool CanClearAllCaches()
        {
            return _cacheManager.GetType() == typeof(AbpMemoryCacheManager);
        }
    }
}